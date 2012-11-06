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
    public class Exchange_Model : ViewModelBase<Exchange_Model>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性

        public Exchange_Model()
        {
            
        }

        public Exchange_Model(TradeGameData gameDataModel)
        {
            GameData = gameDataModel;
        }


        public TradeGameData GameData
        {
            get { return m_GameDataLocator(this).Value; }
            set { m_GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property TradeGameData GameData Setup
        protected Property<TradeGameData> m_GameData =
          new Property<TradeGameData> { LocatorFunc = m_GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<TradeGameData>> m_GameDataLocator =
            RegisterContainerLocator<TradeGameData>(
                "GameData",
                model =>
                {
                    model.m_GameData =
                        model.m_GameData
                        ??
                        new Property<TradeGameData> { LocatorFunc = m_GameDataLocator };
                    return model.m_GameData.Container =
                        model.m_GameData.Container
                        ??
                        new ValueContainer<TradeGameData>("GameData", model);
                });
        #endregion

    }

}

