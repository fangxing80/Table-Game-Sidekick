using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.ViewModels;

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

        public ScoreBoard_Model(TradeGameData_Model gameDataModel)
        {
            GameData = gameDataModel;
        }

        public TradeGameData_Model GameData
        {
            get { return m_GameDataLocator(this).Value; }
            protected set { m_GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property TradeGameData_Model GameData Setup
        protected Property<TradeGameData_Model> m_GameData =
          new Property<TradeGameData_Model> { LocatorFunc = m_GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<TradeGameData_Model>> m_GameDataLocator =
            RegisterContainerLocator<TradeGameData_Model>(
                "GameData",
                model =>
                {
                    model.m_GameData =
                        model.m_GameData
                        ??
                        new Property<TradeGameData_Model> { LocatorFunc = m_GameDataLocator };
                    return model.m_GameData.Container =
                        model.m_GameData.Container
                        ??
                        new ValueContainer<TradeGameData_Model>("GameData", model);
                });
        #endregion



    }

}

