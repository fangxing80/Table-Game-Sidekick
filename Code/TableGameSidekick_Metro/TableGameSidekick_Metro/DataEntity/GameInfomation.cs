using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;

namespace TableGameSidekick_Metro.DataEntity
{
    [DataContract]
    public class GameInfomation : BindableBase<GameInfomation>
    {
        [DataMember]
        public ImageData Image
        {
            get { return _ImageLocator(this).Value; }
            set { _ImageLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ImageData Image Setup
        protected Property<ImageData> _Image =
          new Property<ImageData> { LocatorFunc = _ImageLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ImageData>> _ImageLocator =
            RegisterContainerLocator<ImageData>(
                "Image",
                model =>
                {
                    model._Image =
                        model._Image
                        ??
                        new Property<ImageData> { LocatorFunc = _ImageLocator };
                    return model._Image.Container =
                        model._Image.Container
                        ??
                        new ValueContainer<ImageData>("Image", model);
                });
        #endregion




        [DataMember]

        public string GameDescription
        {
            get { return _GameDescriptionLocator(this).Value; }
            set { _GameDescriptionLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property string GameDescription Setup

        protected Property<string> _GameDescription =
          new Property<string> { LocatorFunc = _GameDescriptionLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> _GameDescriptionLocator =
            RegisterContainerLocator<string>(
                "GameDescription",
                model =>
                {
                    model._GameDescription =
                        model._GameDescription
                        ??
                        new Property<string> { LocatorFunc = _GameDescriptionLocator };
                    return model._GameDescription.Container =
                        model._GameDescription.Container
                        ??
                        new ValueContainer<string>("GameDescription", model);
                });

        #endregion



        [DataMember]
        public Dictionary<string, object> Settings
        {
            get { return _SettingsLocator(this).Value; }
            set { _SettingsLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Dictionary<string,object> Settings Setup
        protected Property<Dictionary<string, object>> _Settings =
          new Property<Dictionary<string, object>> { LocatorFunc = _SettingsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Dictionary<string, object>>> _SettingsLocator =
            RegisterContainerLocator<Dictionary<string, object>>(
                "Settings",
                model =>
                {
                    model._Settings =
                        model._Settings
                        ??
                        new Property<Dictionary<string, object>> { LocatorFunc = _SettingsLocator };
                    return model._Settings.Container =
                        model._Settings.Container
                        ??
                        new ValueContainer<Dictionary<string, object>>("Settings", model);
                });
        #endregion









        [DataMember]

        public DateTime StartTime
        {
            get { return _StartTimeLocator(this).Value; }
            set { _StartTimeLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property DateTime StartTime Setup

        protected Property<DateTime> _StartTime =
          new Property<DateTime> { LocatorFunc = _StartTimeLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<DateTime>> _StartTimeLocator =
            RegisterContainerLocator<DateTime>(
            "StartTime",
            model =>
            {
                model._StartTime =
                    model._StartTime
                    ??
                    new Property<DateTime> { LocatorFunc = _StartTimeLocator };
                return model._StartTime.Container =
                    model._StartTime.Container
                    ??
                    new ValueContainer<DateTime>("StartTime", model, DateTime.Now);
            });

        #endregion









        [DataMember]

        public DateTime LastEditTime
        {
            get { return _LastEditTimeLocator(this).Value; }
            set { _LastEditTimeLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property DateTime LastEditTime Setup

        protected Property<DateTime> _LastEditTime =
          new Property<DateTime> { LocatorFunc = _LastEditTimeLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<DateTime>> _LastEditTimeLocator =
            RegisterContainerLocator<DateTime>(
            "LastEditTime",
            model =>
            {
                model._LastEditTime =
                    model._LastEditTime
                    ??
                    new Property<DateTime> { LocatorFunc = _LastEditTimeLocator };
                return model._LastEditTime.Container =
                    model._LastEditTime.Container
                    ??
                    new ValueContainer<DateTime>("LastEditTime", model,DateTime.Now  );
            });

        #endregion












        [DataMember]

        public ObservableCollection<PlayerInfomation> Players
        {
            get { return _PlayersLocator(this).Value; }
            set { _PlayersLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property ObservableCollection<PlayerInfomation> Players Setup

        protected Property<ObservableCollection<PlayerInfomation>> _Players =
          new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = _PlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerInfomation>>> _PlayersLocator =
            RegisterContainerLocator<ObservableCollection<PlayerInfomation>>(
            "Players",
            model =>
            {
                model._Players =
                    model._Players
                    ??
                    new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = _PlayersLocator };
                return model._Players.Container =
                    model._Players.Container
                    ??
                    new ValueContainer<ObservableCollection<PlayerInfomation>>("Players", model, new ObservableCollection<PlayerInfomation>());
            });

        #endregion











        [DataMember]

        public Guid Id
        {
            get { return _IdLocator(this).Value; }
            set { _IdLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property Guid Id Setup

        protected Property<Guid> _Id =
          new Property<Guid> { LocatorFunc = _IdLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Guid>> _IdLocator =
            RegisterContainerLocator<Guid>(
            "Id",
            model =>
            {
                model._Id =
                    model._Id
                    ??
                    new Property<Guid> { LocatorFunc = _IdLocator };
                return model._Id.Container =
                    model._Id.Container
                    ??
                    new ValueContainer<Guid>("Id", model, Guid.NewGuid());
            });

        #endregion










        [DataMember]

        public GameType GameType
        {
            get { return _GameTypeLocator(this).Value; }
            set { _GameTypeLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameType GameType Setup

        protected Property<GameType> _GameType =
          new Property<GameType> { LocatorFunc = _GameTypeLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameType>> _GameTypeLocator =
            RegisterContainerLocator<GameType>(
            "GameType",
            model =>
            {
                model._GameType =
                    model._GameType
                    ??
                    new Property<GameType> { LocatorFunc = _GameTypeLocator };
                return model._GameType.Container =
                    model._GameType.Container
                    ??
                    new ValueContainer<GameType>("GameType", model);
            });

        #endregion






        [DataMember]

        public string AdvanceGameKey
        {
            get { return _AdvanceGameKeyLocator(this).Value; }
            set { _AdvanceGameKeyLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property string AdvanceGameKey Setup

        protected Property<string> _AdvanceGameKey =
          new Property<string> { LocatorFunc = _AdvanceGameKeyLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> _AdvanceGameKeyLocator =
            RegisterContainerLocator<string>(
            "AdvanceGameKey",
            model =>
            {
                model._AdvanceGameKey =
                    model._AdvanceGameKey
                    ??
                    new Property<string> { LocatorFunc = _AdvanceGameKeyLocator };
                return model._AdvanceGameKey.Container =
                    model._AdvanceGameKey.Container
                    ??
                    new ValueContainer<string>("AdvanceGameKey", model);
            });

        #endregion












    }
}
