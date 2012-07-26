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





        


        public ObservableCollection<GameInfomation> Games
        {
            get { return m_GamesContainerLocator(this).Value; }
            set { m_GamesContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<GameInfomation> Games Setup
        protected PropertyContainer<ObservableCollection<GameInfomation>> m_Games;
        protected static Func<object, PropertyContainer<ObservableCollection<GameInfomation>>> m_GamesContainerLocator =
            RegisterContainerLocator<ObservableCollection<GameInfomation>>(
                "Games",
                model =>
                    model.m_Games =
                        model.m_Games
                        ??
                        new PropertyContainer<ObservableCollection<GameInfomation>>("Games"));
        #endregion


        public GameInfomation SelectedGame
        {
            get { return m_SelectedGameContainerLocator(this).Value; }
            set { m_SelectedGameContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation SelectedGame Setup
        protected PropertyContainer<GameInfomation> m_SelectedGame;
        protected static Func<object, PropertyContainer<GameInfomation>> m_SelectedGameContainerLocator =
            RegisterContainerLocator<GameInfomation>(
                "SelectedGame",
                model =>
                    model.m_SelectedGame =
                        model.m_SelectedGame
                        ??
                        new PropertyContainer<GameInfomation>("SelectedGame"));
        #endregion







        private CommandModel<ReactiveCommand, string> m_NewGameCommand
            = new ReactiveCommand(true)
            .CreateCommandModel("NewGameCommand");

        public CommandModel<ReactiveCommand, string> NewGameCommand
        {
            get
            {
                return m_NewGameCommand;
            }

        }


        private CommandModel<ReactiveCommand, string> m_ContinueCommand
            = new ReactiveCommand(false)
            .CreateCommandModel("ContinueCommand");

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
