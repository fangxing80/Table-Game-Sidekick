using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVVMSidekick.EventRouter;
using MVVMSidekick.Reactive;
using MVVMSidekick.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using System.Reactive.Linq;
using System.Reactive;

using MVVMSidekick.Views;
using MVVMSidekick.Storages;
namespace TableGameSidekick_Metro.ViewModels
{
    public class Start_Model : ViewModelBase<Start_Model>
    {

        CancellationTokenSource _CancellationTokenSource;
        public Start_Model()
        {
            ConfigProperties();

            ConfigCommands();
        }

        private void ConfigProperties()
        {
            _CancellationTokenSource = new CancellationTokenSource();
            _CancellationTokenSource.DisposeWith(this);


            this.Games = new ObservableCollection<GameInfomation>
            {
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
            };
        }

        public Start_Model(IStorage<Dictionary<Guid, GameInfomation>> gameInfoStorage)
        {
            // TODO: Complete member initialization
            this._GameInfoStorage = gameInfoStorage;
            RefreshDataFromStorages();
            ConfigCommands();
        }


        private async void RefreshDataFromStorages()
        {
            await _GameInfoStorage.Refresh();
            if (_GameInfoStorage.Value != null)
            {
                this.Games = new ObservableCollection<GameInfomation>(
                    _GameInfoStorage.Value.OrderByDescending(g => g.Value.LastEditTime)
                    .Select(x => x.Value));
            }


        }




        public String P1
        {
            get { return _P1Locator(this).Value; }
            set { _P1Locator(this).SetValueAndTryNotify(value); }
        }

        #region Property String P1 Setup
        protected Property<String> _P1 =
          new Property<String> { LocatorFunc = _P1Locator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<String>> _P1Locator =
            RegisterContainerLocator<String>(
                "P1",
                model =>
                {
                    model._P1 =
                        model._P1
                        ??
                        new Property<String> { LocatorFunc = _P1Locator };
                    return model._P1.Container =
                        model._P1.Container
                        ??
                        new ValueContainer<String>("P1", model);
                });
        #endregion

        
        public ObservableCollection<GameInfomation> Games
        {
            get { return _GamesLocator(this).Value; }
            set { _GamesLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<GameInfomation> Games Setup
        protected Property<ObservableCollection<GameInfomation>> _Games = new Property<ObservableCollection<GameInfomation>> { LocatorFunc = _GamesLocator };
        static Func<BindableBase, ValueContainer<ObservableCollection<GameInfomation>>> _GamesLocator = RegisterContainerLocator<ObservableCollection<GameInfomation>>("Games", model => model.Initialize("Games", ref model._Games, ref _GamesLocator, _GamesDefaultValueFactory));
        static Func<ObservableCollection<GameInfomation>> _GamesDefaultValueFactory = null;
        #endregion


        





        

        public CommandModel<ReactiveCommand, String> SomeCommand
        {
            get { return _SomeCommand.WithViewModel(this); }
            protected set { _SomeCommand = value; }
        }       
        
        #region SomeCommand Configuration
        [System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _SomeCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion
        
        
         public CommandModel<ReactiveCommand, String> CommandSomeCommand
        {
            get { return _CommandSomeCommandLocator(this).Value; }
            set { _CommandSomeCommandLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property CommandModel<ReactiveCommand, String> CommandSomeCommand Setup        
        protected Property<CommandModel<ReactiveCommand, String>> _CommandSomeCommand = new Property<CommandModel<ReactiveCommand, String>>{ LocatorFunc = _CommandSomeCommandLocator};
        static Func<BindableBase,ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandSomeCommandLocator= RegisterContainerLocator<CommandModel<ReactiveCommand, String>>("CommandSomeCommand", model =>model.Initialize("CommandSomeCommand",ref model._CommandSomeCommand, ref _CommandSomeCommandLocator,_CommandSomeCommandDefaultValueFactory));
        static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandSomeCommandDefaultValueFactory =
            model => new ReactiveCommand(canExecute: true) { ViewModel = model }.CreateCommandModel("SomeCommand")
            ;
        #endregion





        public GameInfomation SelectedGame
        {
            get { return _SelectedGameLocator(this).Value; }
            set { _SelectedGameLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameInfomation SelectedGame Setup

        protected Property<GameInfomation> _SelectedGame =
          new Property<GameInfomation> { LocatorFunc = _SelectedGameLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> _SelectedGameLocator =
            RegisterContainerLocator<GameInfomation>(
            "SelectedGame",
            model =>
            {
                model._SelectedGame =
                    model._SelectedGame
                    ??
                    new Property<GameInfomation> { LocatorFunc = _SelectedGameLocator };
                return model._SelectedGame.Container =
                    model._SelectedGame.Container
                    ??
                    new ValueContainer<GameInfomation>("SelectedGame", model);
            });

        #endregion







        private IStorage<Dictionary<Guid, GameInfomation>> _GameInfoStorage;






        public CommandModel<ReactiveCommand, string> NewGameCommand
        {
            get
            {
                return _NewGameCommand.WithViewModel(this);
            }

        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private CommandModel<ReactiveCommand, string> _NewGameCommand
            = new ReactiveCommand(true)
            .CreateCommandModel("NewGameCommand");



        public CommandModel<ReactiveCommand, string> ContinueCommand
        {
            get
            {
                return _ContinueCommand;
            }

        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private CommandModel<ReactiveCommand, string> _ContinueCommand
            = new ReactiveCommand(false)
            .CreateCommandModel("ContinueCommand");


        void ConfigCommands()
        {
            //主界面的新游戏按钮按下后
            NewGameCommand
                .CommandCore
                .Subscribe
                (
                  async _ =>
                  {
                      //第一步使用名叫NewGame的view 创建一个 newgame对象 创建后返回主界面
                      var newGame = await Navigator.FrameNavigate<GameInfomation>(
                                  TableGameSidekick_Metro.Constants.ViewTypes.NewGame,
                                   this,
                                   null
                           );
                      //如果创建成功 就前往游戏主体View
                      if (newGame != null)
                      {
                          await Navigator.FrameNavigate(
                                     TableGameSidekick_Metro.Constants.ViewTypes.GamePlay,
                                     this,
                                    new Dictionary<string, object> { { NavigateParameterKeys.GameInfomation_ChosenGame, newGame } }

                          );
                      }
                      //否则（取消的话）返回主界面
                  }
                )
                .DisposeWith(this); //主界面被dispose时注销此事件


            this.GetValueContainer(x => x.SelectedGame)
                .GetValueChangedObservable()
                .Select(e => e.EventArgs != null)
                .Subscribe(_ContinueCommand.CommandCore.CanExecuteObserver)
                .DisposeWith(this);


            ContinueCommand.CommandCore
                .Subscribe
                (
                    async _ =>

                       await App.MainFrame.GetFrameNavigator().FrameNavigate(
                            TableGameSidekick_Metro.Constants.ViewTypes.GamePlay,
                            this,
                         new Dictionary<string, Object>() 
                            {
                                
                                {NavigateParameterKeys.GameInfomation_ChosenGame,this.SelectedGame}
                            }
                        )
                )
                .DisposeWith(this);


        }












    }
}
