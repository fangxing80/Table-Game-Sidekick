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
using TableGameSidekick_Metro.Common;
using TableGameSidekick_Metro.ViewModels;
using Windows.Storage;

namespace TableGameSidekick_Metro
{
    sealed partial class App
    {
        public static string[] PresavedPics = new string[]
       {
            "ms-appx:///Assets/Icons/Armour.png",
            "ms-appx:///Assets/Icons/BusinessMan.png",
            "ms-appx:///Assets/Icons/Car.png",
            "ms-appx:///Assets/Icons/Countingmachine.png",
            "ms-appx:///Assets/Icons/Globe.png",
            "ms-appx:///Assets/Icons/Gold.png",
            "ms-appx:///Assets/Icons/Key.png",
            "ms-appx:///Assets/Icons/MedicalBag.png",
            "ms-appx:///Assets/Icons/Negative.png",
            "ms-appx:///Assets/Icons/Positive.png",
            "ms-appx:///Assets/Icons/Pound.png",
            "ms-appx:///Assets/Icons/Recycle.png",
            "ms-appx:///Assets/Icons/StockIndexUp.png",
            "ms-appx:///Assets/Icons/Wheelchair.png",
       };
        public static class DefaultViewModelKeys
        {
            public static readonly string DefaultTypedViewModelName = "Model";

        }
        public static class NavigateParameterKeys
        {
            public static readonly string ViewInitActionName = "InitAction";

        }

        public static class Views
        {

            public static readonly string Start = typeof(Start).FullName;
            public static readonly string NewGame = typeof(NewGame).FullName;
            public static readonly string GamePlay = typeof(GamePlay).FullName;
            public static readonly string SelectPlayers = typeof(SelectPlayers).FullName;

            public static Dictionary<string, Action<LayoutAwarePage>>
            SaveStateActions = new Dictionary<string, Action<LayoutAwarePage>>
            {

            };

            public static Dictionary<string, Action<LayoutAwarePage>>
                PageInitActions = new Dictionary<string, Action<LayoutAwarePage>> 
                { 
                    {
                        Start , 
                        ( p=>
                        {
                            var st=Storages.Instance.GameInfomationsStorage;

                            var vm = new Start_Model(st);
                      
                            p.DefaultViewModel = vm;
                    
                        })
                    },
                    {
                        NewGame , 
                        ( p=>
                        {
                            
                    
                        })
                    },
                    {
                        GamePlay , 
                        ( p=>
                        {
                            
                    
                        })
                    },
                    {
                        SelectPlayers ,
                        p=>
                            {}

                    
                    }
                };
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

            public Storages()
            {

              
            }


            public static Storages Instance = new Storages();

            public CollectionStorage<GameInfomation> GameInfomationsStorage = new CollectionStorage<GameInfomation>("GameInfomations.json");
            public Dictionary<Guid, IStorage<GameData>> GameDatasStorages = new Dictionary<Guid, IStorage<GameData>>();
            public CollectionStorage<PlayerInfomation> PlayerInfomationStorage = new CollectionStorage<PlayerInfomation>("PlayerInfomations.json");
            public List<StorageFile> PlayerImages;


        }


    }
}
