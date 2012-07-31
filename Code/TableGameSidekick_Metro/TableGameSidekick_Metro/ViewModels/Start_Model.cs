using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVVM.EventRouter;
using MVVM.Reactive;
using MVVM.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using System.Reactive.Linq;
using System.Reactive;
namespace TableGameSidekick_Metro.ViewModels
{
    public class Start_Model : ViewModelBase<Start_Model>
    {

        CancellationTokenSource m_CancellationTokenSource;
        public Start_Model()
        {
            m_CancellationTokenSource = new CancellationTokenSource();
            m_CancellationTokenSource.RegisterDispose(this);


            this.Games = new ObservableCollection<GameInfomation>
            {
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
                new GameInfomation { Id=Guid.NewGuid (), LastEditTime=DateTime.Now ,  StartTime=DateTime.Now     },
            };

            ConfigCommands();
        }

        public Start_Model(Storages.CollectionStorage<GameInfomation> gameInfoStorage)
        {
            // TODO: Complete member initialization
            this.m_GameInfoStorage = gameInfoStorage;
            RefreshDataFromStorages();

        }


        private async void RefreshDataFromStorages()
        {
            await m_GameInfoStorage.Refresh();
            this.Games = new ObservableCollection<GameInfomation>(
                m_GameInfoStorage.Value.OrderByDescending(g => g.LastEditTime));

        }





        public ObservableCollection<GameInfomation> Games
        {
            get { return m_Games.Locate(this).Value; }
            set { m_Games.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<GameInfomation>  Games Setup
        protected Property<ObservableCollection<GameInfomation>> m_Games = new Property<ObservableCollection<GameInfomation>>(m_GamesLocator);
        static Func<ViewModelBase, ValueContainer<ObservableCollection<GameInfomation>>> m_GamesLocator =
            RegisterContainerLocator<ObservableCollection<GameInfomation>>(
                "Games",
                model =>
                    model.m_Games.Container =
                        model.m_Games.Container
                        ??
                        new ValueContainer<ObservableCollection<GameInfomation>>("Games", model));
        #endregion









        public GameInfomation SelectedGame
        {
            get { return m_SelectedGame.Locate(this).Value; }
            set { m_SelectedGame.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation SelectedGame Setup
        protected Property<GameInfomation> m_SelectedGame = new Property<GameInfomation>(m_SelectedGameLocator);
        static Func<ViewModelBase, ValueContainer<GameInfomation>> m_SelectedGameLocator =
            RegisterContainerLocator<GameInfomation>(
                "SelectedGame",
                model =>
                    model.m_SelectedGame.Container =
                        model.m_SelectedGame.Container
                        ??
                        new ValueContainer<GameInfomation>("SelectedGame", model));
        #endregion














        private CommandModel<ReactiveCommand, string> m_NewGameCommand
            = new ReactiveCommand(true)
            .CreateCommandModel("NewGameCommand");

        public CommandModel<ReactiveCommand, string> NewGameCommand
        {
            get
            {
                return m_NewGameCommand.WithViewModel(this);
            }

        }


        private CommandModel<ReactiveCommand, string> m_ContinueCommand
            = new ReactiveCommand(false)
            .CreateCommandModel("ContinueCommand");
        private Storages.CollectionStorage<GameInfomation> m_GameInfoStorage;

        public CommandModel<ReactiveCommand, string> ContinueCommand
        {
            get
            {
                return m_ContinueCommand;
            }

        }


        void ConfigCommands()
        {
            m_NewGameCommand
                .ConfigCommandCore(
                    core =>
                    {
                        core
                            .Subscribe
                            (
                                _ =>
                                    App.MainEventRouter.RaiseEvent(
                                    this,
                                    new NavigateCommandEventArgs()
                                    {
                                        SourceViewId = App.Views.MainPage,
                                        TargetViewId = App.Views.NewGame
                                    })
                            )
                            .RegisterDispose(this);
                    }
                );

            m_ContinueCommand
                .ConfigCommandCore(
                    core =>
                    {
                        this.GetPropertyContainer(x => x.SelectedGame)
                            .GetValueChangeObservable()
                            .Select(e =>
                                e.EventArgs != null)
                            .Subscribe(core.CanExecuteObserver)
                            .RegisterDispose(this);


                        core
                            .Subscribe
                            (
                                _ =>
                                    App.MainEventRouter.RaiseEvent(
                                    this,
                                    new NavigateCommandEventArgs()
                                    {
                                        SourceViewId = App.Views.MainPage,
                                        TargetViewId = App.Views.GamePlay,
                                        ParameterDictionary = new Dictionary<string, Object>() 
                                        {
                                            {App.Views.MainPage_NavigateParameters.bool_IsNewGame ,false},
                                            {App.Views.MainPage_NavigateParameters.GameInfomation_ChosenGame,this.SelectedGame}
                                        }
                                    })
                            )
                            .RegisterDispose(this);

                    });


        }












    }
}
