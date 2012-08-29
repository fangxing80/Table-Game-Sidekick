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
namespace TableGameSidekick_Metro.ViewModels
{
    public class NewGame_Model : ViewModelBase<NewGame_Model>
    {

        public NewGame_Model()
        {

            ConfigModel();
        }

        private async void ConfigModel()
        {
            GameInfomationPrototypes = new ObservableCollection<DataEntity.GameInfomation>();

            //加入三种默认游戏规则
            await AddGameType(GameType.ScoreGame);
            await AddGameType(GameType.StopwatchGame);
            await AddGameType(GameType.TradeGame);

            //Todo:将来有更多游戏规则 在这里加入


            //下面配置内部选中逻辑
            //选中游戏信息原型变化后，其部分信息被直接填充到NewGameInfo.
            m_SelectedPrototypeGameInfomation.Locate(this)
                .GetValueChangeObservable()
                .Where(e => e.EventArgs != null)
                .Subscribe
                (
                    e =>
                    {
                        var s = e.EventArgs;
                        this.NewGameInfomation.GameType = s.GameType;
                        this.NewGameInfomation.Image = s.Image;
                        this.NewGameInfomation.GameDescription = s.GameDescription;
                        this.NewGameInfomation.LastEditTime = DateTime.Now;
                        this.NewGameInfomation.StartTime = DateTime.Now;

                    }

                )
                .RegisterDispose(this);

            ConfigCommands();


        }

        private void ConfigCommands()
        {

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
                            var stream = rnds.AsStreamForRead(); ;
                            var bts = new byte[rnds.Size];
                            await stream.ReadAsync(bts, 0, bts.Length);
                            mayAdd.Image = new ImageData { ByteArray = bts };
                            vm.NewGameInfomation.Players.Add(mayAdd);


                        }

                    }
                )
                .RegisterDispose(this);

            //删除已选玩家按钮必须在已选玩家大于0的时候启用
            m_SelectedPlayersLocator(this)
                .GetValueChangeObservable()
                .Select(x => x.EventArgs.Count > 0)
                .Subscribe(
                    DeleteSelectedPlayersCommand
                    .CommandCore
                    .CanExecuteObserver)
                .RegisterDispose(this); ;

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
        }

        private async Task AddGameType(GameType t)
        {
            byte[] imgdata;
            var fl = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/GameType/" + t.ToString()+".png"));
            using (var stream = await fl.OpenReadAsync())
            {
                var istrm = new MemoryStream();
                var dnstream = stream.AsStreamForRead();
                await dnstream.CopyToAsync(istrm);
                istrm.Position = 0;
                imgdata = istrm.ToArray();
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
        static Func<ViewModelBase, ValueContainer<ObservableCollection<GameInfomation>>> m_GameInfomationPrototypesLocator =
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
            get { return m_SelectedPrototypeGameInfomation.Locate(this).Value; }
            set { m_SelectedPrototypeGameInfomation.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation SelectedPrototypeGameInfomation Setup
        protected Property<GameInfomation> m_SelectedPrototypeGameInfomation = new Property<GameInfomation> { LocatorFunc = m_SelectedPrototypeGameInfomationLocator };
        static Func<ViewModelBase, ValueContainer<GameInfomation>> m_SelectedPrototypeGameInfomationLocator =
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
                         new ValueContainer<GameInfomation>("SelectedPrototypeGameInfomation", new GameInfomation() { Id = Guid.NewGuid() }, model);

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
        static Func<ViewModelBase, ValueContainer<GameInfomation>> m_NewGameInfomationLocator =
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
                        new ValueContainer<GameInfomation>("NewGameInfomation", new GameInfomation(), model);
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
        static Func<ViewModelBase, ValueContainer<IList<object>>> m_SelectedPlayersLocator =
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






        CommandModel<ReactiveCommand, string> m_StartGameCommand
            = new ReactiveCommand().CreateCommandModel("StartGameCommand");
        public CommandModel<ReactiveCommand, string> StartGameCommand
        {
            get { return m_StartGameCommand.WithViewModel(this); }
            protected set { m_StartGameCommand = value; }
        }


        CommandModel<ReactiveCommand, String> m_PickContactsCommand
            = new ReactiveCommand(true).CreateCommandModel("AddPlayersCommand");

        public CommandModel<ReactiveCommand, String> PickContactsCommand
        {
            get { return m_PickContactsCommand.WithViewModel(this); }
            protected set { m_PickContactsCommand = value; }
        }



        public CommandModel<ReactiveCommand, String> DeleteSelectedPlayersCommand
        {
            get { return m_DeleteSelectedPlayersCommand.WithViewModel(this); }
            protected set { m_DeleteSelectedPlayersCommand = value; }
        }

        #region DeleteSelectedPlayersCommand Configuration
        CommandModel<ReactiveCommand, String> m_DeleteSelectedPlayersCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("DeleteSelectedPlayers");
        #endregion


    }
}
