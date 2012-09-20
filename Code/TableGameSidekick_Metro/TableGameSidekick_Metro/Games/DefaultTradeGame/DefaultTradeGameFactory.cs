using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableGameSidekick_Metro.Common;
using TableGameSidekick_Metro.DataEntity;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Models;
using Windows.UI.Xaml.Controls;
using MVVMSidekick.ViewModels;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame
{
    public class DefaultTradeGameFactory : GameFactoryBase
    {
        async public override Task<LayoutAwarePage> CreateGame(GameInfomation gameInfomation)
        {
           
            TradeGameData vm;
            var storage = new Storages.Storage<TradeGameData>(GetSaveFileName(gameInfomation));

            await storage.Refresh();

            if (storage.Value == null)
            {
                storage.Value = new TradeGameData(storage, gameInfomation);
            }
            vm = storage.Value;






            var rval = new DefaultTradeGame.Views.TradeGamePage() { DefaultViewModel = vm };

            return rval;

        }

        

    }
}
