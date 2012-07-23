using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVVM.Reactive;
using MVVM.ViewModels;
using TableGameSidekick_Metro.DataEntity;
namespace TableGameSidekick_Metro.ViewModels
{
    public class Start_Model : ViewModelBase<Start_Model>
    {

        CancellationTokenSource m_CancellationTokenSource;
        public Start_Model()
        {
            m_CancellationTokenSource = new CancellationTokenSource();
            m_CancellationTokenSource.RegisterDispose(this);

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




        


        CommandModel<ReactiveAsyncCommand, string> m_NewGameCommand
            = new CommandModel<ReactiveAsyncCommand, string>
            (
                new ReactiveAsyncCommand(),
                "NewGameCommand"
            );

        public CommandModel<ReactiveAsyncCommand, string> NewGameCommand
        {
            get { return m_NewGameCommand; }
            set { m_NewGameCommand = value; }
        }

        CommandModel<ReactiveAsyncCommand, string> m_ContinueCommand
           = new CommandModel<ReactiveAsyncCommand, string>
           (
               new ReactiveAsyncCommand(),
               "ContinueCommand"
           );

        public CommandModel<ReactiveAsyncCommand, string> ContinueCommand
        {
            get { return m_ContinueCommand; }
            set { m_ContinueCommand = value; }
        }











    }
}
