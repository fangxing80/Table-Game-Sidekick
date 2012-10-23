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
            get { return m_ImageLocator(this).Value; }
            set { m_ImageLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ImageData Image Setup
        protected Property<ImageData> m_Image =
          new Property<ImageData> { LocatorFunc = m_ImageLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ImageData>> m_ImageLocator =
            RegisterContainerLocator<ImageData>(
                "Image",
                model =>
                {
                    model.m_Image =
                        model.m_Image
                        ??
                        new Property<ImageData> { LocatorFunc = m_ImageLocator };
                    return model.m_Image.Container =
                        model.m_Image.Container
                        ??
                        new ValueContainer<ImageData>("Image", model);
                });
        #endregion




        [DataMember]

        public string GameDescription
        {
            get { return m_GameDescriptionLocator(this).Value; }
            set { m_GameDescriptionLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property string GameDescription Setup

        protected Property<string> m_GameDescription =
          new Property<string> { LocatorFunc = m_GameDescriptionLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> m_GameDescriptionLocator =
            RegisterContainerLocator<string>(
                "GameDescription",
                model =>
                {
                    model.m_GameDescription =
                        model.m_GameDescription
                        ??
                        new Property<string> { LocatorFunc = m_GameDescriptionLocator };
                    return model.m_GameDescription.Container =
                        model.m_GameDescription.Container
                        ??
                        new ValueContainer<string>("GameDescription", model);
                });

        #endregion



        [DataMember]
        public Dictionary<string, object> Settings
        {
            get { return m_SettingsLocator(this).Value; }
            set { m_SettingsLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Dictionary<string,object> Settings Setup
        protected Property<Dictionary<string, object>> m_Settings =
          new Property<Dictionary<string, object>> { LocatorFunc = m_SettingsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Dictionary<string, object>>> m_SettingsLocator =
            RegisterContainerLocator<Dictionary<string, object>>(
                "Settings",
                model =>
                {
                    model.m_Settings =
                        model.m_Settings
                        ??
                        new Property<Dictionary<string, object>> { LocatorFunc = m_SettingsLocator };
                    return model.m_Settings.Container =
                        model.m_Settings.Container
                        ??
                        new ValueContainer<Dictionary<string, object>>("Settings", model);
                });
        #endregion









        [DataMember]

        public DateTime StartTime
        {
            get { return m_StartTimeLocator(this).Value; }
            set { m_StartTimeLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property DateTime StartTime Setup

        protected Property<DateTime> m_StartTime =
          new Property<DateTime> { LocatorFunc = m_StartTimeLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<DateTime>> m_StartTimeLocator =
            RegisterContainerLocator<DateTime>(
            "StartTime",
            model =>
            {
                model.m_StartTime =
                    model.m_StartTime
                    ??
                    new Property<DateTime> { LocatorFunc = m_StartTimeLocator };
                return model.m_StartTime.Container =
                    model.m_StartTime.Container
                    ??
                    new ValueContainer<DateTime>("StartTime", model);
            });

        #endregion









        [DataMember]

        public DateTime LastEditTime
        {
            get { return m_LastEditTimeLocator(this).Value; }
            set { m_LastEditTimeLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property DateTime LastEditTime Setup

        protected Property<DateTime> m_LastEditTime =
          new Property<DateTime> { LocatorFunc = m_LastEditTimeLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<DateTime>> m_LastEditTimeLocator =
            RegisterContainerLocator<DateTime>(
            "LastEditTime",
            model =>
            {
                model.m_LastEditTime =
                    model.m_LastEditTime
                    ??
                    new Property<DateTime> { LocatorFunc = m_LastEditTimeLocator };
                return model.m_LastEditTime.Container =
                    model.m_LastEditTime.Container
                    ??
                    new ValueContainer<DateTime>("LastEditTime", model);
            });

        #endregion












        [DataMember]

        public ObservableCollection<PlayerInfomation> Players
        {
            get { return m_PlayersLocator(this).Value; }
            set { m_PlayersLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property ObservableCollection<PlayerInfomation> Players Setup

        protected Property<ObservableCollection<PlayerInfomation>> m_Players =
          new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = m_PlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerInfomation>>> m_PlayersLocator =
            RegisterContainerLocator<ObservableCollection<PlayerInfomation>>(
            "Players",
            model =>
            {
                model.m_Players =
                    model.m_Players
                    ??
                    new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = m_PlayersLocator };
                return model.m_Players.Container =
                    model.m_Players.Container
                    ??
                    new ValueContainer<ObservableCollection<PlayerInfomation>>("Players", model, new ObservableCollection<PlayerInfomation>());
            });

        #endregion











        [DataMember]

        public Guid Id
        {
            get { return m_IdLocator(this).Value; }
            set { m_IdLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property Guid Id Setup

        protected Property<Guid> m_Id =
          new Property<Guid> { LocatorFunc = m_IdLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Guid>> m_IdLocator =
            RegisterContainerLocator<Guid>(
            "Id",
            model =>
            {
                model.m_Id =
                    model.m_Id
                    ??
                    new Property<Guid> { LocatorFunc = m_IdLocator };
                return model.m_Id.Container =
                    model.m_Id.Container
                    ??
                    new ValueContainer<Guid>("Id", model, Guid.NewGuid());
            });

        #endregion










        [DataMember]

        public GameType GameType
        {
            get { return m_GameTypeLocator(this).Value; }
            set { m_GameTypeLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameType GameType Setup

        protected Property<GameType> m_GameType =
          new Property<GameType> { LocatorFunc = m_GameTypeLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameType>> m_GameTypeLocator =
            RegisterContainerLocator<GameType>(
            "GameType",
            model =>
            {
                model.m_GameType =
                    model.m_GameType
                    ??
                    new Property<GameType> { LocatorFunc = m_GameTypeLocator };
                return model.m_GameType.Container =
                    model.m_GameType.Container
                    ??
                    new ValueContainer<GameType>("GameType", model);
            });

        #endregion






        [DataMember]

        public string AdvanceGameKey
        {
            get { return m_AdvanceGameKeyLocator(this).Value; }
            set { m_AdvanceGameKeyLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property string AdvanceGameKey Setup

        protected Property<string> m_AdvanceGameKey =
          new Property<string> { LocatorFunc = m_AdvanceGameKeyLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> m_AdvanceGameKeyLocator =
            RegisterContainerLocator<string>(
            "AdvanceGameKey",
            model =>
            {
                model.m_AdvanceGameKey =
                    model.m_AdvanceGameKey
                    ??
                    new Property<string> { LocatorFunc = m_AdvanceGameKeyLocator };
                return model.m_AdvanceGameKey.Container =
                    model.m_AdvanceGameKey.Container
                    ??
                    new ValueContainer<string>("AdvanceGameKey", model);
            });

        #endregion












    }
}
