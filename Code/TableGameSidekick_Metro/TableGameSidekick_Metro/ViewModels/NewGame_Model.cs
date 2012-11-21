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

using MVVMSidekick.EventRouter;
using System.Collections.Specialized;
using MVVMSidekick.Storages;
namespace TableGameSidekick_Metro.ViewModels
{
    public class NewGame_Model : ViewModelBase<NewGame_Model, GameInfomation>
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
            _Storage = storage;
        }
        IStorage<Dictionary<Guid, GameInfomation>> _Storage;

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
            _SelectedPrototypeGameInfomation.LocateValueContainer(this)
                .GetValueChangedObservable()
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
                .DisposeWith(this);

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
            .DisposeWith(this);
            #endregion

            #region 设置删除已选玩家 Command

            _SelectedPlayersLocator(this)
                .GetValueChangedObservable()
                .Select(x => x.EventArgs.Count > 0)//删除已选玩家按钮必须在已选玩家大于0的时候启用
                .Subscribe(
                    DeleteSelectedPlayersCommand
                    .CommandCore
                    .CanExecuteObserver)
                .DisposeWith(this); ;

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
                    _SelectedPrototypeGameInfomation.LocateValueContainer(this)
                    .GetValueChangedObservable()
                    .Select(x => 1)

                )
                .Select(x => NewGameInfomation.Players.Count >= 2 && (NewGameInfomation.AdvanceGameKey != string.Empty || NewGameInfomation.GameType == GameType.Advanced))
                .Subscribe
                (
                    SaveGameInfoAndStartCommand.CommandCore.CanExecuteObserver
                )
                .DisposeWith(this);

            SaveGameInfoAndStartCommand.CommandCore
                .Subscribe(
                     async e =>
                     {
                         if (_Storage.Value ==null)
                         {
                             _Storage.Value = new Dictionary<Guid, GameInfomation>();
                         }
                         _Storage.Value[NewGameInfomation.Id] = NewGameInfomation;
                         await _Storage.Save();
                         this.Result = NewGameInfomation;
                         this.Close();
                     }
                )
                .DisposeWith(this);
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
            get { return _GameInfomationPrototypesLocator(this).Value; }
            set { _GameInfomationPrototypesLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<GameInfomation> GameInfomationPrototypes Setup
        protected Property<ObservableCollection<GameInfomation>> _GameInfomationPrototypes =
            new Property<ObservableCollection<GameInfomation>> { LocatorFunc = _GameInfomationPrototypesLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<GameInfomation>>> _GameInfomationPrototypesLocator =
            RegisterContainerLocator<ObservableCollection<GameInfomation>>(
            "GameInfomationPrototypes",
            model =>
            {
                model._GameInfomationPrototypes =
                    model._GameInfomationPrototypes
                    ??
                    new Property<ObservableCollection<GameInfomation>> { LocatorFunc = _GameInfomationPrototypesLocator };
                return model._GameInfomationPrototypes.Container =
                    model._GameInfomationPrototypes.Container
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
            get { return _SelectedPrototypeGameInfomation.LocateValueContainer(this).Value; }
            set { _SelectedPrototypeGameInfomation.LocateValueContainer(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation SelectedPrototypeGameInfomation Setup
        protected Property<GameInfomation> _SelectedPrototypeGameInfomation = new Property<GameInfomation> { LocatorFunc = _SelectedPrototypeGameInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> _SelectedPrototypeGameInfomationLocator =
            RegisterContainerLocator<GameInfomation>(
            "SelectedPrototypeGameInfomation",
            model =>
            {
                model._SelectedPrototypeGameInfomation =
                    model._SelectedPrototypeGameInfomation
                    ??
                    new Property<GameInfomation> { LocatorFunc = _SelectedPrototypeGameInfomationLocator };
                return model._SelectedPrototypeGameInfomation.Container =
                     model._SelectedPrototypeGameInfomation.Container
                     ??
                     new ValueContainer<GameInfomation>("SelectedPrototypeGameInfomation", model, new GameInfomation() { Id = Guid.NewGuid() });

            });
        #endregion







        /// <summary>
        /// 最终产生GameInfomation
        /// </summary>

        public GameInfomation NewGameInfomation
        {
            get { return _NewGameInfomationLocator(this).Value; }
            set { _NewGameInfomationLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameInfomation NewGameInfomation Setup

        protected Property<GameInfomation> _NewGameInfomation =
          new Property<GameInfomation> { LocatorFunc = _NewGameInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> _NewGameInfomationLocator =
            RegisterContainerLocator<GameInfomation>(
            "NewGameInfomation",
            model =>
            {
                model._NewGameInfomation =
                    model._NewGameInfomation
                    ??
                    new Property<GameInfomation> { LocatorFunc = _NewGameInfomationLocator };
                return model._NewGameInfomation.Container =
                    model._NewGameInfomation.Container
                    ??
                    new ValueContainer<GameInfomation>("NewGameInfomation", model, new GameInfomation());
            });

        #endregion





        public IList<object> SelectedPlayers
        {
            get { return _SelectedPlayersLocator(this).Value; }
            set { _SelectedPlayersLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property IList<object> SelectedPlayers Setup
        protected Property<IList<object>> _SelectedPlayers =
          new Property<IList<object>> { LocatorFunc = _SelectedPlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<IList<object>>> _SelectedPlayersLocator =
            RegisterContainerLocator<IList<object>>(
            "SelectedPlayers",
            model =>
            {
                model._SelectedPlayers =
                    model._SelectedPlayers
                    ??
                    new Property<IList<object>> { LocatorFunc = _SelectedPlayersLocator };
                return model._SelectedPlayers.Container =
                    model._SelectedPlayers.Container
                    ??
                    new ValueContainer<IList<object>>("SelectedPlayers", model);
            });
        #endregion






        public CommandModel<ReactiveCommand, string> StartGameCommand
        {
            get { return _StartGameCommand.WithViewModel(this); }
            protected set { _StartGameCommand = value; }
        }
        #region StartGameCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, string> _StartGameCommand
            = new ReactiveCommand().CreateCommandModel("StartGameCommand");

        #endregion


        public CommandModel<ReactiveCommand, String> PickContactsCommand
        {
            get { return _PickContactsCommand.WithViewModel(this); }
            protected set { _PickContactsCommand = value; }
        }

        #region PickContactsCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _PickContactsCommand
      = new ReactiveCommand(true).CreateCommandModel("AddPlayersCommand");

        #endregion

        public CommandModel<ReactiveCommand, String> DeleteSelectedPlayersCommand
        {
            get { return _DeleteSelectedPlayersCommand.WithViewModel(this); }
            protected set { _DeleteSelectedPlayersCommand = value; }
        }

        #region DeleteSelectedPlayersCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _DeleteSelectedPlayersCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("DeleteSelectedPlayers");
        #endregion




        public CommandModel<ReactiveCommand, String> SaveGameInfoAndStartCommand
        {
            get { return _SaveGameInfoAndStartCommand.WithViewModel(this); }
            protected set { _SaveGameInfoAndStartCommand = value; }
        }

        #region SaveGameInfoAndStartCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _SaveGameInfoAndStartCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("SaveGameInfoAndStartCommand");
        #endregion



    }
}
