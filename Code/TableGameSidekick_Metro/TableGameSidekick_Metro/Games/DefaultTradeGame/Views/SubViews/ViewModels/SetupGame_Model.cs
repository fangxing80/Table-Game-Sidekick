using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews.ViewModels
{

    [DataContract]
    public class SetupGame_Model : ViewModelBase<SetupGame_Model>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性


        
        public DefaultTradeGame.Models.TradeGameData_Model GameData
        {
            get { return m_GameDataLocator(this).Value; }
            set { m_GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property DefaultTradeGame.Models.TradeGameData_Model GameData Setup
        protected Property<DefaultTradeGame.Models.TradeGameData_Model> m_GameData =
          new Property<DefaultTradeGame.Models.TradeGameData_Model> { LocatorFunc = m_GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<DefaultTradeGame.Models.TradeGameData_Model>> m_GameDataLocator =
            RegisterContainerLocator<DefaultTradeGame.Models.TradeGameData_Model>(
                "GameData",
                model =>
                {
                    model.m_GameData =
                        model.m_GameData
                        ??
                        new Property<DefaultTradeGame.Models.TradeGameData_Model> { LocatorFunc = m_GameDataLocator };
                    return model.m_GameData.Container =
                        model.m_GameData.Container
                        ??
                        new ValueContainer<DefaultTradeGame.Models.TradeGameData_Model>("GameData", model);
                });
        #endregion

    }
	
}



