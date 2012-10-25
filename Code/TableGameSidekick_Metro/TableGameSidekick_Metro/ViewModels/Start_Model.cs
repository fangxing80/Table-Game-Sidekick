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
using TableGameSidekick_Metro.Storages;
using MVVMSidekick.Views;
namespace TableGameSidekick_Metro.ViewModels
{
    public class Start_Model : ViewModelBase<Start_Model>
    {

        CancellationTokenSource m_CancellationTokenSource;
        public Start_Model()
        {
            ConfigProperties();

            ConfigCommands();
        }

        private void ConfigProperties()
        {
            m_CancellationTokenSource = new CancellationTokenSource();
            m_CancellationTokenSource.RegisterDisposeToViewModel(this);


            this.Games = new ObservableCollection<GameInfomation>
            {
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
            };
        }

        public Start_Model(Storages.IStorage<Dictionary<Guid, GameInfomation>> gameInfoStorage)
        {
            // TODO: Complete member initialization
            this.m_GameInfoStorage = gameInfoStorage;
            RefreshDataFromStorages();
            ConfigCommands();
        }


        private async void RefreshDataFromStorages()
        {
            await m_GameInfoStorage.Refresh();
            this.Games = new ObservableCollection<GameInfomation>(
                m_GameInfoStorage.Value.OrderByDescending(g => g.Value.LastEditTime)
                .Select(x => x.Value));

        }






        public ObservableCollection<GameInfomation> Games
        {
            get { return m_GamesLocator(this).Value; }
            set { m_GamesLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property ObservableCollection<GameInfomation> Games Setup

        protected Property<ObservableCollection<GameInfomation>> m_Games =
          new Property<ObservableCollection<GameInfomation>> { LocatorFunc = m_GamesLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<GameInfomation>>> m_GamesLocator =
            RegisterContainerLocator<ObservableCollection<GameInfomation>>(
            "Games",
            model =>
            {
                model.m_Games =
                    model.m_Games
                    ??
                    new Property<ObservableCollection<GameInfomation>> { LocatorFunc = m_GamesLocator };
                return model.m_Games.Container =
                    model.m_Games.Container
                    ??
                    new ValueContainer<ObservableCollection<GameInfomation>>("Games", model);
            });

        #endregion










        public GameInfomation SelectedGame
        {
            get { return m_SelectedGameLocator(this).Value; }
            set { m_SelectedGameLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameInfomation SelectedGame Setup

        protected Property<GameInfomation> m_SelectedGame =
          new Property<GameInfomation> { LocatorFunc = m_SelectedGameLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> m_SelectedGameLocator =
            RegisterContainerLocator<GameInfomation>(
            "SelectedGame",
            model =>
            {
                model.m_SelectedGame =
                    model.m_SelectedGame
                    ??
                    new Property<GameInfomation> { LocatorFunc = m_SelectedGameLocator };
                return model.m_SelectedGame.Container =
                    model.m_SelectedGame.Container
                    ??
                    new ValueContainer<GameInfomation>("SelectedGame", model);
            });

        #endregion







        private IStorage<Dictionary<Guid, GameInfomation>> m_GameInfoStorage;






        public CommandModel<ReactiveCommand, string> NewGameCommand
        {
            get
            {
                return m_NewGameCommand.WithViewModel(this);
            }

        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private CommandModel<ReactiveCommand, string> m_NewGameCommand
            = new ReactiveCommand(true)
            .CreateCommandModel("NewGameCommand");



        public CommandModel<ReactiveCommand, string> ContinueCommand
        {
            get
            {
                return m_ContinueCommand;
            }

        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private CommandModel<ReactiveCommand, string> m_ContinueCommand
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
                                   Constants.ViewsNames.NewGame,
                                   null
                           );
                      //如果创建成功 就前往游戏主体View
                      if (newGame != null)
                      {
                          await Navigator.FrameNavigate(
                                     Constants.ViewsNames.GamePlay,
                                    new Dictionary<string, object> { { NavigateParameterKeys.GameInfomation_ChosenGame, newGame } }

                          );
                      }
                      //否则（取消的话）返回主界面
                  }
                )
                .RegisterDisposeToViewModel(this); //主界面被dispose时注销此事件


            this.GetValueContainer(x => x.SelectedGame)
                .GetValueChangeObservable()
                .Select(e => e.EventArgs != null)
                .Subscribe(m_ContinueCommand.CommandCore.CanExecuteObserver)
                .RegisterDisposeToViewModel(this);


            ContinueCommand.CommandCore
                .Subscribe
                (
                    async _ =>

                       await App.MainFrame.GetFrameNavigator().FrameNavigate(
                            Constants.ViewsNames.GamePlay,
                         new Dictionary<string, Object>() 
                            {
                                
                                {NavigateParameterKeys.GameInfomation_ChosenGame,this.SelectedGame}
                            }
                        )
                )
                .RegisterDisposeToViewModel(this);


        }












    }
}
