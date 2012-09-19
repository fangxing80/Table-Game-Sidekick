using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TableGameSidekick_Metro.Games.DefaultTradeGame
{
    public class DefaultTradeGameDataFactory:GameDataFactoryBase
    {
        public override Page CreateGame()
        {
            return new DefaultTradeGame.Views.TradeGamePage();
        }
    }
}
