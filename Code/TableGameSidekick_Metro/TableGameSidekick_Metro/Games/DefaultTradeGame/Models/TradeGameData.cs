using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TableGameSidekick_Metro.Common;
using TableGameSidekick_Metro.DataEntity;
using TableGameSidekick_Metro.ViewModels;
using System.Collections.ObjectModel;
using TableGameSidekick_Metro.Storages;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{
    [DataContract]
    public class TradeGameData : ViewModelBase<TradeGameData>
    {
        public TradeGameData()

        { }

        public TradeGameData(IStorage<TradeGameData> storage, GameInfomation gameInfomation)
        {
            m_GameInfomation = gameInfomation;
            m_Storage = storage;


            Players = new ObservableDictionary<string, PlayerInfomation>(
                     gameInfomation.Players.ToDictionary(x => x.Name, x => x));
        }


        GameInfomation m_GameInfomation;

        IStorage<TradeGameData> m_Storage;

        public ObservableDictionary<String, PlayerInfomation> Players
        {
            get { return m_PlayersLocator(this).Value; }
            set { m_PlayersLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableDictionary <String,PlayerInfomation>  Players Setup
        protected Property<ObservableDictionary<String, PlayerInfomation>> m_Players =
          new Property<ObservableDictionary<String, PlayerInfomation>> { LocatorFunc = m_PlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<ObservableDictionary<String, PlayerInfomation>>> m_PlayersLocator =
            RegisterContainerLocator<ObservableDictionary<String, PlayerInfomation>>(
                "Players",
                model =>
                {
                    model.m_Players =
                        model.m_Players
                        ??
                        new Property<ObservableDictionary<String, PlayerInfomation>> { LocatorFunc = m_PlayersLocator };
                    return model.m_Players.Container =
                        model.m_Players.Container
                        ??
                        new ValueContainer<ObservableDictionary<String, PlayerInfomation>>("Players", model);
                });
        #endregion

        [DataMember]
        public ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>> PlayerResources
        {
            get { return m_PlayerResourcesLocator(this).Value; }
            set { m_PlayerResourcesLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property  ObservableDictionary<String,ObservableDictionary<string,ResourcesEntry >> PlayerResources Setup
        protected Property<ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>>> m_PlayerResources =
          new Property<ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>>> { LocatorFunc = m_PlayerResourcesLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>>>> m_PlayerResourcesLocator =
            RegisterContainerLocator<ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>>>(
                "PlayerResources",
                model =>
                {
                    model.m_PlayerResources =
                        model.m_PlayerResources
                        ??
                        new Property<ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>>> { LocatorFunc = m_PlayerResourcesLocator };
                    return model.m_PlayerResources.Container =
                        model.m_PlayerResources.Container
                        ??
                        new ValueContainer<ObservableDictionary<String, ObservableDictionary<string, ResourcesEntry>>>("PlayerResources", model);
                });
        #endregion


        [DataMember]
        public ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>> ResourceLogs
        {
            get { return m_ResourceLogsLocator(this).Value; }
            set { m_ResourceLogsLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection< Tuple<String,List<ResourcesEntry> ,DateTime> ResourceLogs Setup
        protected Property<ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>>> m_ResourceLogs =
          new Property<ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>>> { LocatorFunc = m_ResourceLogsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>>>> m_ResourceLogsLocator =
            RegisterContainerLocator<ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>>>(
                "ResourceLogs",
                model =>
                {
                    model.m_ResourceLogs =
                        model.m_ResourceLogs
                        ??
                        new Property<ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>>> { LocatorFunc = m_ResourceLogsLocator };
                    return model.m_ResourceLogs.Container =
                        model.m_ResourceLogs.Container
                        ??
                        new ValueContainer<ObservableCollection<Tuple<String, List<ResourcesEntry>, DateTime>>>("ResourceLogs", model);
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
        static Func<ViewModelBase, ValueContainer<bool>> m_IsStartedLocator =
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
