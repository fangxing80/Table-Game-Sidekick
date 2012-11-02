using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVVMSidekick.EventRouter;
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
using MVVMSidekick.Reactive;
using TableGameSidekick_Metro.Storages;
using TableGameSidekick_Metro.DataEntity;

using TableGameSidekick_Metro.ViewModels;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using TableGameSidekick_Metro.Games;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace TableGameSidekick_Metro
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        static Frame m_MainFrame;
        /// <summary>
        /// 本App的主Frame
        /// </summary>
        public static Frame MainFrame
        {
            get { return App.m_MainFrame; }
            set { App.m_MainFrame = value; }
        }
        /// <summary>
        /// 本App的主事件路由
        /// </summary>
        public static EventRouter MainEventRouter = EventRouter.Instance;


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

 
            MainEventRouter.InitFrameNavigator(ref  m_MainFrame);
            MainFrame.GetFrameNavigator().PageInitActions = new Dictionary<Type, Action<MVVMSidekick.Views.LayoutAwarePage, IDictionary<string, object>>> 
                { 
                    {
                        typeof (Start) , 
                        ( (p,pars)=>
                        {
                            var st = Constants.Storages.Instance.GameInfomationsStorage;

                            var vm = new Start_Model(st);
                      
                            p.DefaultViewModel = vm;
                    
                        })
                    },
                    {
                      typeof (  NewGame ), 
                        ( (p,pars)=>
                        {
                            var vm = new NewGame_Model(Constants.Storages.Instance.GameInfomationsStorage);
                            p.DefaultViewModel = vm;
                    
                        })
                    },
                    {
                        typeof (  GamePlay) , 
                        ( async (p,pars)=>
                        {

                            var gi=pars[NavigateParameterKeys.GameInfomation_ChosenGame] as GameInfomation;
                            var gameKey=gi.GameType == GameType.Advanced ? gi.AdvanceGameKey :gi.GameType.ToString ();
                            var fac = Constants.Games.Factories[gameKey] as GameFactoryBase;
                            var game = await fac.CreateGame(gi);
                            p.DefaultViewModel = new GamePlay_Model()
                            {
                                GameData  = (BindableBase)game.DefaultViewModel,
                                CurrentGameInfomation = gi,                                
                            };



                            var gplayp = p as GamePlay;
                            gplayp.GamePage = game;


                        })
                    },
                    {
                        typeof (SelectPlayers) ,
                        (p,pars)=>
                            {}

                    
                    }
                };

        }

        static App()
        {

        }


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


            if (!MainFrame.Navigate(typeof(Start),
                null
                ))
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

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.Kind == ActivationKind.ContactPicker)
            {
                var page = new SelectPlayers();
                Window.Current.Content = page;
                var vm = new SelectPlayers_Model(((ContactPickerActivatedEventArgs)args).ContactPickerUI, Constants.Storages.Instance.PlayerInfomationStorage, Constants.PresavedPics);
                page.DefaultViewModel = vm;
                Window.Current.Activate();
            }
        }
    }
}
