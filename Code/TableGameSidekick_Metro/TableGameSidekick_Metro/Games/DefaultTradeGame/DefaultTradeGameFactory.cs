using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableGameSidekick_Metro.DataEntity;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Models;
using Windows.UI.Xaml.Controls;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews.ViewModels;
using MVVMSidekick.EventRouter;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.ViewModels;
using MVVMSidekick.Storages;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame
{
    public class DefaultTradeGameFactory : GameFactoryBase
    {

        static DefaultTradeGameFactory()
        {
            TradeGamePage_Model.ExchangeViewType = typeof(Exchange);
            TradeGamePage_Model.SetupGameViewType = typeof(SetupGame);
            TradeGamePage_Model.ScoreBoardViewType = typeof(ScoreBoard);
            TradeGamePage_Model.TradeGamePageViewType = typeof(TradeGamePage);

        }

        async public override Task<LayoutAwarePage> CreateGameAndNavigateTo(GameInfomation gameInfomation, Frame targetFrame)
        {


            TradeGamePage_Model vm;
            var storage = new Storage<TradeGameData>(GetSaveFileName(gameInfomation));

            await storage.Refresh();


            vm = new TradeGamePage_Model(storage, gameInfomation);


            EventRouter.Instance.InitFrameNavigator(ref targetFrame);

            var navigator = vm.Navigator = targetFrame.GetFrameNavigator();

            navigator.PageInitActions
                = new Dictionary<Type, Action<LayoutAwarePage, IDictionary<string, object>>> 
                {
                    {
                        typeof (SetupGame),
                        (p,dic)=>
                            {
                                var svm = new SetupGame_Model(vm.GameData);
                                p.DefaultViewModel = svm;
                            }

                    },
                               
                    {
                        typeof (Exchange),
                        (p,dic)=>
                            {
                                var svm = new SetupGame_Model(vm.GameData);
                                p.DefaultViewModel = svm;
                            
                            }

                    },

                     {
                        typeof (ScoreBoard),
                        (p,dic)=>
                            {
                                var svm = new ScoreBoard_Model(vm.GameData);
                                p.DefaultViewModel = svm;
                            
                            }

                    },

                    {
                        typeof (TradeGamePage),
                        (p,dic)=>
                            {
                               
                                p.DefaultViewModel = vm;
                                vm.CommandOnLoadCommand.Execute(null);
                            }

                    },
                };


            await navigator.FrameNavigate(Views.ViewModels.TradeGamePage_Model.TradeGamePageViewType, vm, null);



            return targetFrame.Content as LayoutAwarePage;

        }



    }
}
