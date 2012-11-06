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
namespace TableGameSidekick_Metro.Games.DefaultTradeGame
{
    public class DefaultTradeGameFactory : GameFactoryBase
    {
        async public override Task<LayoutAwarePage> CreateGame(GameInfomation gameInfomation, Frame targetFrame)
        {

            TradeGameData_Model.ExchangeViewType = typeof(Exchange);
            TradeGameData_Model.SetupGameViewType = typeof(SetupGame);
            TradeGameData_Model vm;
            var storage = new Storages.Storage<TradeGameData_Model>(GetSaveFileName(gameInfomation));

            await storage.Refresh();

            if (storage.Value == null)
            {
                storage.Value = new TradeGameData_Model(storage, gameInfomation);
            }
            vm = storage.Value;
            
            
            EventRouter.Instance.InitFrameNavigator(ref targetFrame);

            var navigator = vm.Navigator = targetFrame.GetFrameNavigator();
            
            navigator.PageInitActions
                = new Dictionary<Type, Action<LayoutAwarePage, IDictionary<string, object>>> 
                {
                    {
                        typeof (SetupGame),
                        (p,dic)=>
                            {
                                var svm = new SetupGame_Model(vm);
                                p.DefaultViewModel = svm;
                            }

                    },
                               
                    {
                        typeof (Exchange),
                        (p,dic)=>
                            {
                                var svm = new SetupGame_Model(vm);
                                p.DefaultViewModel = svm;
                            
                            }

                    },

                     {
                        typeof (ScoreBoard),
                        (p,dic)=>
                            {
                                var svm = new ScoreBoard_Model(vm);
                                p.DefaultViewModel = svm;
                            
                            }

                    },
                };




            var rval = new DefaultTradeGame.Views.TradeGamePage() { DefaultViewModel = vm };
            return rval;

        }



    }
}
