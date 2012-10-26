using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using TableGameSidekick_Metro.DataEntity;
using TableGameSidekick_Metro.Common;


namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{
    public class PlayerData : BindableBase<PlayerData>
    {

        public PlayerInfomation PlayerInfomation
        {
            get { return m_PlayerInfomationLocator(this).Value; }
            set { m_PlayerInfomationLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property PlayerInfomation PlayerInfomation Setup
        protected Property<PlayerInfomation> m_PlayerInfomation =
          new Property<PlayerInfomation> { LocatorFunc = m_PlayerInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<PlayerInfomation>> m_PlayerInfomationLocator =
            RegisterContainerLocator<PlayerInfomation>(
                "PlayerInfomation",
                model =>
                {
                    model.m_PlayerInfomation =
                        model.m_PlayerInfomation
                        ??
                        new Property<PlayerInfomation> { LocatorFunc = m_PlayerInfomationLocator };
                    return model.m_PlayerInfomation.Container =
                        model.m_PlayerInfomation.Container
                        ??
                        new ValueContainer<PlayerInfomation>("PlayerInfomation", model);
                });
        #endregion




        public ObservableCollection<ResourcesEntry> Resources
        {
            get { return m_ResourcesLocator(this).Value; }
            set { m_ResourcesLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<ResourcesEntry >  Resources Setup
        protected Property<ObservableCollection<ResourcesEntry>> m_Resources =
          new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = m_ResourcesLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourcesEntry>>> m_ResourcesLocator =
            RegisterContainerLocator<ObservableCollection<ResourcesEntry>>(
                "Resources",
                model =>
                {
                    model.m_Resources =
                        model.m_Resources
                        ??
                        new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = m_ResourcesLocator };
                    return model.m_Resources.Container =
                        model.m_Resources.Container
                        ??
                        new ValueContainer<ObservableCollection<ResourcesEntry>>("Resources", model);
                });
        #endregion





    }
}
