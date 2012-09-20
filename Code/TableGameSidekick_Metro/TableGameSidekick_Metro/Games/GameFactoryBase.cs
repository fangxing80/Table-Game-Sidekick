using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableGameSidekick_Metro.Common;
using TableGameSidekick_Metro.DataEntity;
using Windows.UI.Xaml.Controls;

namespace TableGameSidekick_Metro.Games
{
    public abstract class GameFactoryBase
    {
        public abstract Task<LayoutAwarePage> CreateGame(GameInfomation gameInfomation);

        protected static string GetSaveFileName(GameInfomation gameInfomation)
        {
            return gameInfomation.Id.ToString() + "GameData.json";
        }
        

    }
}
