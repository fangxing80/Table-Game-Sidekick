using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVVM.EventRouter;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MVVM.Reactive;
using TableGameSidekick_Metro.Storages;
using TableGameSidekick_Metro.DataEntity;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace TableGameSidekick_Metro
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Frame MainFrame;
        public static class Views
        {
            public static readonly string MainPage = typeof(MainPage).FullName;
            public static readonly string Start = typeof(Start).FullName;
            public static readonly string NewGame = typeof(NewGame).FullName;
            public static readonly string GamePlay = typeof(GamePlay).FullName;

            //static Dictionary<string, Lazy<Page>> viewCache
            //    = new Dictionary<string, Lazy<Page>> 
            //    { 
            //        {MainPage , new Lazy<Page>( ()=>new MainPage ()) },
            //        {Start,new Lazy<Page>( ()=>new Start () { ViewModel = new ViewModels.Start_Model ()}   )  },
            //        {NewGame ,new Lazy<Page>( ()=>new NewGame  ()) },
            //        { GamePlay ,new Lazy<Page>( ()=>new GamePlay ())},
            //    };


            //public static Page GetViewFromCache(string name)
            //{
            //    return viewCache[name].Value;
            //}

            public static class MainPage_NavigateParameters
            {

                public static readonly string GameInfomation_ChosenGame = "GameInfomation_ChosenGame";
                public static readonly string bool_IsNewGame = "bool_IsNewGame";

            }
        }
        public class Storages
        {
            public CollectionStorage<GameInfomation> GameInfomationsStorage = new CollectionStorage<GameInfomation>("GameInfomations.json");
            public Dictionary<Guid, IStorage<GameData>> GameDatasStorages = new Dictionary<Guid, IStorage<GameData>>();
            public CollectionStorage<PlayerInfomation> PlayerInfomationStorage = new CollectionStorage<PlayerInfomation>("PlayerInfomations.json");
            
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            var noneedDispose = MainEventRouter.GetEventObject<NavigateCommandEventArgs>().GetRouterEventObservable()
                .Subscribe(
                    ep =>
                    {
                        MainFrame.Navigate(Type.GetType(ep.EventArgs.TargetViewId));
                    }
                );



        }

        public static EventRouter MainEventRouter = EventRouter.Instance;


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Create a Frame to act navigation context and navigate to the first page
            MainFrame = new Frame();
            if (!MainFrame.Navigate(typeof(Start)))
            {
                throw new Exception("Failed to create initial page");
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = MainFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
