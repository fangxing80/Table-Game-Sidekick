using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using MVVMSidekick.Reactive;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;
using TableGameSidekick_Metro.Storages;
using MVVMSidekick.EventRouter;
using System.Collections.Specialized;
namespace TableGameSidekick_Metro.ViewModels
{
    public class NewGame_Model : ViewModelBase<NewGame_Model,GameInfomation>
    {
        public NewGame_Model()
        {
            ConfigProperties()
                .ContinueWith(
                _ =>
                ConfigCommands()
            );
        }
        public NewGame_Model(IStorage<Dictionary<Guid, GameInfomation>> storage)
            : this()
        {
            m_Storage = storage;
        }
        IStorage<Dictionary<Guid, GameInfomation>> m_Storage;

        private async Task ConfigProperties()
        {
            GameInfomationPrototypes = new ObservableCollection<DataEntity.GameInfomation>();

            //加入三种默认游戏规则
            //Todo:将来有更多游戏规则 在这里加入
            #region 加入三种默认游戏规则
            await AddGameType(GameType.ScoreGame);
            await AddGameType(GameType.StopwatchGame);
            await AddGameType(GameType.TradeGame);
            #endregion


            ////选中游戏信息原型变化后，其部分信息被直接填充到NewGameInfo.
            #region 配置游戏原型选中后的逻辑

            //选中游戏信息原型变化后，其部分信息被直接填充到NewGameInfo.
            m_SelectedPrototypeGameInfomation.LocateValueContainer(this)
                .GetValueChangeObservable()
                .Where(e => e.EventArgs != null)
                .Subscribe
                (
                    e =>
                    {
                        this.NewGameInfomation = this.NewGameInfomation ?? new GameInfomation();
                        var s = e.EventArgs;
                        this.NewGameInfomation.GameType = s.GameType;
                        this.NewGameInfomation.Image = s.Image;
                        this.NewGameInfomation.GameDescription = s.GameDescription;
                        this.NewGameInfomation.LastEditTime = DateTime.Now;
                        this.NewGameInfomation.StartTime = DateTime.Now;

                    }

                )
                .RegisterDisposeToViewModel(this);

            #endregion



        }

        private void ConfigCommands()
        {


            #region 设置选择用户的 Command
            PickContactsCommand.CommandCore
            .Subscribe
            (
                async e =>
                {
                    var vm = ((NewGame_Model)e.EventArgs.ViewModel);
                    var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
                    contactPicker.CommitButtonText = "Select";
                    var contacts = await contactPicker.PickMultipleContactsAsync();
                    vm.NewGameInfomation.Players = vm.NewGameInfomation.Players ?? new ObservableCollection<PlayerInfomation>();
                    foreach (var c in contacts)
                    {

                        var mayAdd = new PlayerInfomation()
                        {
                            Name = c.Name,
                        };

                        //因为已经重写过 PlayerInfomation的Equal 所以只要名字相同就覆盖（也就是先去除再添加）
                        if (vm.NewGameInfomation.Players.Contains(mayAdd))
                        {
                            vm.NewGameInfomation.Players.Remove(mayAdd);
                        }
                        var rnds = await c.GetThumbnailAsync();
                        if (rnds != null)
                        {


                            var stream = rnds.AsStreamForRead();

                            var bts = new byte[rnds.Size];
                            await stream.ReadAsync(bts, 0, bts.Length);
                            mayAdd.Image = new ImageData { ByteArray = bts };
                        }
                        vm.NewGameInfomation.Players.Add(mayAdd);

                    }

                }
            )
            .RegisterDisposeToViewModel(this);
            #endregion

            #region 设置删除已选玩家 Command

            m_SelectedPlayersLocator(this)
                .GetValueChangeObservable()
                .Select(x => x.EventArgs.Count > 0)//删除已选玩家按钮必须在已选玩家大于0的时候启用
                .Subscribe(
                    DeleteSelectedPlayersCommand
                    .CommandCore
                    .CanExecuteObserver)
                .RegisterDisposeToViewModel(this); ;

            DeleteSelectedPlayersCommand.CommandCore
                .Subscribe(
                    e =>
                    {
                        if (SelectedPlayers != null)
                        {
                            foreach (PlayerInfomation item in this.SelectedPlayers)
                            {
                                this.NewGameInfomation.Players
                                    .Remove(item);

                            }
                        }
                    }

                );
            #endregion

            #region 设置保存并开始游戏 Command
            //只有选择了至少2个玩家 并且选择了游戏原型才能开始游戏
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
                (
                   eh => NewGameInfomation.Players.CollectionChanged += eh,
                   eh => NewGameInfomation.Players.CollectionChanged -= eh
                )
                .Select(_ => 0
                    )
                .Concat
                (
                    m_SelectedPrototypeGameInfomation.LocateValueContainer(this)
                    .GetValueChangeObservable()
                    .Select(x => 1)

                )
                .Select(x => NewGameInfomation.Players.Count >= 2 && (NewGameInfomation.AdvanceGameKey != string.Empty || NewGameInfomation.GameType == GameType.Advanced))
                .Subscribe
                (
                    SaveGameInfoAndStartCommand.CommandCore.CanExecuteObserver
                )
                .RegisterDisposeToViewModel(this);

            SaveGameInfoAndStartCommand.CommandCore
                .Subscribe(
                     async e =>
                     {
                         m_Storage.Value[NewGameInfomation.Id] = NewGameInfomation;
                         await m_Storage.Save();
                         this.Result = NewGameInfomation;
                         this.Close();
                     }
                )
                .RegisterDisposeToViewModel(this);
            #endregion
        }

        private async Task AddGameType(GameType t)
        {
            byte[] imgdata = new byte[0];
            StorageFile fl = null;
            var flTask = Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/GameType/" + t.ToString() + ".png")).AsTask();
            try
            {
                fl = flTask.Result;
            }
            catch (Exception)
            {


            }


            if (fl != null)
            {

                using (var stream = await fl.OpenReadAsync())
                {
                    var istrm = new MemoryStream();
                    var dnstream = stream.AsStreamForRead();
                    await dnstream.CopyToAsync(istrm);
                    istrm.Position = 0;
                    imgdata = istrm.ToArray();
                }
            }
            //  var id=new ImageData (){ ByteArray = }
            GameInfomationPrototypes.Add(new GameInfomation()
            {
                AdvanceGameKey = "",
                GameType = t,
                Image = new ImageData { ByteArray = imgdata },
                GameDescription = t.ToString() + "类型游戏"
            }
            );
        }



        /// <summary>
        /// 可选的 GameInfomation原型
        /// </summary>
        public ObservableCollection<GameInfomation> GameInfomationPrototypes
        {
            get { return m_GameInfomationPrototypesLocator(this).Value; }
            set { m_GameInfomationPrototypesLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<GameInfomation> GameInfomationPrototypes Setup
        protected Property<ObservableCollection<GameInfomation>> m_GameInfomationPrototypes =
            new Property<ObservableCollection<GameInfomation>> { LocatorFunc = m_GameInfomationPrototypesLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<GameInfomation>>> m_GameInfomationPrototypesLocator =
            RegisterContainerLocator<ObservableCollection<GameInfomation>>(
            "GameInfomationPrototypes",
            model =>
            {
                model.m_GameInfomationPrototypes =
                    model.m_GameInfomationPrototypes
                    ??
                    new Property<ObservableCollection<GameInfomation>> { LocatorFunc = m_GameInfomationPrototypesLocator };
                return model.m_GameInfomationPrototypes.Container =
                    model.m_GameInfomationPrototypes.Container
                    ??
                    new ValueContainer<ObservableCollection<GameInfomation>>("GameInfomationPrototypes", model);
            }
            );
        #endregion





        /// <summary>
        /// 在View中被选中的GameInfomation原型
        /// </summary>
        public GameInfomation SelectedPrototypeGameInfomation
        {
            get { return m_SelectedPrototypeGameInfomation.LocateValueContainer(this).Value; }
            set { m_SelectedPrototypeGameInfomation.LocateValueContainer(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation SelectedPrototypeGameInfomation Setup
        protected Property<GameInfomation> m_SelectedPrototypeGameInfomation = new Property<GameInfomation> { LocatorFunc = m_SelectedPrototypeGameInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> m_SelectedPrototypeGameInfomationLocator =
            RegisterContainerLocator<GameInfomation>(
            "SelectedPrototypeGameInfomation",
            model =>
            {
                model.m_SelectedPrototypeGameInfomation =
                    model.m_SelectedPrototypeGameInfomation
                    ??
                    new Property<GameInfomation> { LocatorFunc = m_SelectedPrototypeGameInfomationLocator };
                return model.m_SelectedPrototypeGameInfomation.Container =
                     model.m_SelectedPrototypeGameInfomation.Container
                     ??
                     new ValueContainer<GameInfomation>("SelectedPrototypeGameInfomation", model, new GameInfomation() { Id = Guid.NewGuid() });

            });
        #endregion







        /// <summary>
        /// 最终产生GameInfomation
        /// </summary>

        public GameInfomation NewGameInfomation
        {
            get { return m_NewGameInfomationLocator(this).Value; }
            set { m_NewGameInfomationLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameInfomation NewGameInfomation Setup

        protected Property<GameInfomation> m_NewGameInfomation =
          new Property<GameInfomation> { LocatorFunc = m_NewGameInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> m_NewGameInfomationLocator =
            RegisterContainerLocator<GameInfomation>(
            "NewGameInfomation",
            model =>
            {
                model.m_NewGameInfomation =
                    model.m_NewGameInfomation
                    ??
                    new Property<GameInfomation> { LocatorFunc = m_NewGameInfomationLocator };
                return model.m_NewGameInfomation.Container =
                    model.m_NewGameInfomation.Container
                    ??
                    new ValueContainer<GameInfomation>("NewGameInfomation", model, new GameInfomation());
            });

        #endregion





        public IList<object> SelectedPlayers
        {
            get { return m_SelectedPlayersLocator(this).Value; }
            set { m_SelectedPlayersLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property IList<object> SelectedPlayers Setup
        protected Property<IList<object>> m_SelectedPlayers =
          new Property<IList<object>> { LocatorFunc = m_SelectedPlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<IList<object>>> m_SelectedPlayersLocator =
            RegisterContainerLocator<IList<object>>(
            "SelectedPlayers",
            model =>
            {
                model.m_SelectedPlayers =
                    model.m_SelectedPlayers
                    ??
                    new Property<IList<object>> { LocatorFunc = m_SelectedPlayersLocator };
                return model.m_SelectedPlayers.Container =
                    model.m_SelectedPlayers.Container
                    ??
                    new ValueContainer<IList<object>>("SelectedPlayers", model);
            });
        #endregion






        public CommandModel<ReactiveCommand, string> StartGameCommand
        {
            get { return m_StartGameCommand.WithViewModel(this); }
            protected set { m_StartGameCommand = value; }
        }
        #region StartGameCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, string> m_StartGameCommand
            = new ReactiveCommand().CreateCommandModel("StartGameCommand");

        #endregion


        public CommandModel<ReactiveCommand, String> PickContactsCommand
        {
            get { return m_PickContactsCommand.WithViewModel(this); }
            protected set { m_PickContactsCommand = value; }
        }

        #region PickContactsCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_PickContactsCommand
      = new ReactiveCommand(true).CreateCommandModel("AddPlayersCommand");

        #endregion

        public CommandModel<ReactiveCommand, String> DeleteSelectedPlayersCommand
        {
            get { return m_DeleteSelectedPlayersCommand.WithViewModel(this); }
            protected set { m_DeleteSelectedPlayersCommand = value; }
        }

        #region DeleteSelectedPlayersCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_DeleteSelectedPlayersCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("DeleteSelectedPlayers");
        #endregion




        public CommandModel<ReactiveCommand, String> SaveGameInfoAndStartCommand
        {
            get { return m_SaveGameInfoAndStartCommand.WithViewModel(this); }
            protected set { m_SaveGameInfoAndStartCommand = value; }
        }

        #region SaveGameInfoAndStartCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_SaveGameInfoAndStartCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("SaveGameInfoAndStartCommand");
        #endregion



    }
}
