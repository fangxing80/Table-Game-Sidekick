using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TableGameSidekick_Metro.Games
{
    public abstract class GameDataFactoryBase
    {
        public abstract Page CreateGame();

        public IDictionary<string, object> Settings { get; set; }

    }
}
