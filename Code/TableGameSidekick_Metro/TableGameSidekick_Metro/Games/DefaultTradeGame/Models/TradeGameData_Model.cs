using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using TableGameSidekick_Metro.DataEntity;
using TableGameSidekick_Metro.ViewModels;
using System.Collections.ObjectModel;
using TableGameSidekick_Metro.Storages;
using MVVMSidekick.Common;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{
    [DataContract]
    public class TradeGameData_Model : ViewModelBase<TradeGameData_Model>
    {
        public TradeGameData_Model()

        {

        }

        public TradeGameData_Model(IStorage<TradeGameData_Model> storage, GameInfomation gameInfomation)
        {
            m_GameInfomation = gameInfomation;
            m_Storage = storage;


        }
        GameInfomation m_GameInfomation;

        IStorage<TradeGameData_Model> m_Storage;




        
        public ObservableCollection<PlayerData> PlayersData
        {
            get { return m_PlayersDataLocator(this).Value; }
            set { m_PlayersDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<PlayerData> PlayersData Setup
        protected Property<ObservableCollection<PlayerData>> m_PlayersData =
          new Property<ObservableCollection<PlayerData>> { LocatorFunc = m_PlayersDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerData>>> m_PlayersDataLocator =
            RegisterContainerLocator<ObservableCollection<PlayerData>>(
                "PlayersData",
                model =>
                {
                    model.m_PlayersData =
                        model.m_PlayersData
                        ??
                        new Property<ObservableCollection<PlayerData>> { LocatorFunc = m_PlayersDataLocator };
                    return model.m_PlayersData.Container =
                        model.m_PlayersData.Container
                        ??
                        new ValueContainer<ObservableCollection<PlayerData>>("PlayersData", model);
                });
        #endregion



        public bool IsStarted
        {
            get { return m_IsStartedLocator(this).Value; }
            set { m_IsStartedLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property bool IsStarted Setup
        protected Property<bool> m_IsStarted =
          new Property<bool> { LocatorFunc = m_IsStartedLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<bool>> m_IsStartedLocator =
            RegisterContainerLocator<bool>(
                "IsStarted",
                model =>
                {
                    model.m_IsStarted =
                        model.m_IsStarted
                        ??
                        new Property<bool> { LocatorFunc = m_IsStartedLocator };
                    return model.m_IsStarted.Container =
                        model.m_IsStarted.Container
                        ??
                        new ValueContainer<bool>("IsStarted", model);
                });
        #endregion




    }
}
