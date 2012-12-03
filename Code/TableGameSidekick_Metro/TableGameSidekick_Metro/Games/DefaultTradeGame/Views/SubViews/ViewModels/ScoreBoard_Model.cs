using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.ViewModels;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Models;

namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews.ViewModels
{

    [DataContract]
    public class ScoreBoard_Model : ViewModelBase<ScoreBoard_Model>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性

        public ScoreBoard_Model()
        { 
        
        }

        public ScoreBoard_Model( TradeGameData gameData )
        {
            GameData = gameData;
        }

   

        public TradeGameData GameData
        {
            get { return _GameDataLocator(this).Value; }
            set { _GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property TradeGameData GameData Setup
        protected Property<TradeGameData> _GameData =
          new Property<TradeGameData> { LocatorFunc = _GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<TradeGameData>> _GameDataLocator =
            RegisterContainerLocator<TradeGameData>(
                "GameData",
                model =>
                {
                    model._GameData =
                        model._GameData
                        ??
                        new Property<TradeGameData> { LocatorFunc = _GameDataLocator };
                    return model._GameData.Container =
                        model._GameData.Container
                        ??
                        new ValueContainer<TradeGameData>("GameData", model);
                });
        #endregion


    }

}

