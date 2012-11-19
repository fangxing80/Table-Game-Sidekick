using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using TableGameSidekick_Metro.DataEntity;
using System.Runtime.Serialization;



namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{
    [DataContract]
    public class PlayerData : BindableBase<PlayerData>
    {
        [DataMember]
        public PlayerInfomation PlayerInfomation
        {
            get { return _PlayerInfomationLocator(this).Value; }
            set { _PlayerInfomationLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property PlayerInfomation PlayerInfomation Setup
        protected Property<PlayerInfomation> _PlayerInfomation =
          new Property<PlayerInfomation> { LocatorFunc = _PlayerInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<PlayerInfomation>> _PlayerInfomationLocator =
            RegisterContainerLocator<PlayerInfomation>(
                "PlayerInfomation",
                model =>
                {
                    model._PlayerInfomation =
                        model._PlayerInfomation
                        ??
                        new Property<PlayerInfomation> { LocatorFunc = _PlayerInfomationLocator };
                    return model._PlayerInfomation.Container =
                        model._PlayerInfomation.Container
                        ??
                        new ValueContainer<PlayerInfomation>("PlayerInfomation", model);
                });
        #endregion



        [DataMember]
        public ObservableCollection<ResourcesEntry> Resources
        {
            get { return _ResourcesLocator(this).Value; }
            set { _ResourcesLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<ResourcesEntry >  Resources Setup
        protected Property<ObservableCollection<ResourcesEntry>> _Resources =
          new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = _ResourcesLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourcesEntry>>> _ResourcesLocator =
            RegisterContainerLocator<ObservableCollection<ResourcesEntry>>(
                "Resources",
                model =>
                {
                    model._Resources =
                        model._Resources
                        ??
                        new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = _ResourcesLocator };
                    return model._Resources.Container =
                        model._Resources.Container
                        ??
                        new ValueContainer<ObservableCollection<ResourcesEntry>>("Resources", model);
                });
        #endregion





    }
}
