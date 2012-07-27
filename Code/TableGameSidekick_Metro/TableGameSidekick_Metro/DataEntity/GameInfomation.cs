using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModels;

namespace TableGameSidekick_Metro.DataEntity
{
    [DataContract]
    public class GameInfomation : ViewModelBase<GameInfomation>
    {
        [DataMember]

        public Byte[] Image
        {
            get { return m_Image.Locate(this).Value; }
            set { m_Image.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property Byte[] Image Setup
        protected Property<Byte[]> m_Image = new Property<Byte[]>(m_ImageLocator);
        static Func<ViewModelBase, ValueContainer<Byte[]>> m_ImageLocator =
            RegisterContainerLocator<Byte[]>(
                "Image",
                model =>
                    model.m_Image.Container =
                        model.m_Image.Container
                        ??
                        new ValueContainer<Byte[]>("Image", model));
        #endregion










        [DataMember]
        public DateTime StartTime
        {
            get { return m_StartTime.Locate(this).Value; }
            set { m_StartTime.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property DateTime StartTime Setup
        protected Property<DateTime> m_StartTime = new Property<DateTime>(m_StartTimeLocator);
        static Func<ViewModelBase, ValueContainer<DateTime>> m_StartTimeLocator =
            RegisterContainerLocator<DateTime>(
                "StartTime",
                model =>
                    model.m_StartTime.Container =
                        model.m_StartTime.Container
                        ??
                        new ValueContainer<DateTime>("StartTime", model));
        #endregion










        [DataMember]
        public DateTime LastEditTime
        {
            get { return m_LastEditTime.Locate(this).Value; }
            set { m_LastEditTime.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property DateTime  LastEditTime Setup
        protected Property<DateTime> m_LastEditTime = new Property<DateTime>(m_LastEditTimeLocator);
        static Func<ViewModelBase, ValueContainer<DateTime>> m_LastEditTimeLocator =
            RegisterContainerLocator<DateTime>(
                "LastEditTime",
                model =>
                    model.m_LastEditTime.Container =
                        model.m_LastEditTime.Container
                        ??
                        new ValueContainer<DateTime>("LastEditTime", model));
        #endregion











        [DataMember]
        public ObservableCollection<PlayerInfomation> Players
        {
            get { return m_Players.Locate(this).Value; }
            set { m_Players.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<PlayerInfomation> Players Setup
        protected Property<ObservableCollection<PlayerInfomation>> m_Players = new Property<ObservableCollection<PlayerInfomation>>(m_PlayersLocator);
        static Func<ViewModelBase, ValueContainer<ObservableCollection<PlayerInfomation>>> m_PlayersLocator =
            RegisterContainerLocator<ObservableCollection<PlayerInfomation>>(
                "Players",
                model =>
                    model.m_Players.Container =
                        model.m_Players.Container
                        ??
                        new ValueContainer<ObservableCollection<PlayerInfomation>>("Players", model));
        #endregion



        






        [DataMember]
        public Guid Id
        {
            get { return m_Id.Locate(this).Value; }
            set { m_Id.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property Guid Id Setup
        protected Property<Guid> m_Id = new Property<Guid>(m_IdLocator);
        static Func<ViewModelBase, ValueContainer<Guid>> m_IdLocator =
            RegisterContainerLocator<Guid>(
                "Id",
                model =>
                    model.m_Id.Container =
                        model.m_Id.Container
                        ??
                        new ValueContainer<Guid>("Id", model));
        #endregion



        






        [DataMember]
        public GameType GameType
        {
            get { return m_GameType.Locate(this).Value; }
            set { m_GameType.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property GameType GameType Setup
        protected Property<GameType> m_GameType = new Property<GameType>(m_GameTypeLocator);
        static Func<ViewModelBase, ValueContainer<GameType>> m_GameTypeLocator =
            RegisterContainerLocator<GameType>(
                "GameType",
                model =>
                    model.m_GameType.Container =
                        model.m_GameType.Container
                        ??
                        new ValueContainer<GameType>("GameType", model));
        #endregion



        




        [DataMember]
        public string AdvanceGameKey
        {
            get { return m_AdvanceGameKey.Locate(this).Value; }
            set { m_AdvanceGameKey.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property string AdvanceGameKey Setup
        protected Property<string> m_AdvanceGameKey = new Property<string>(m_AdvanceGameKeyLocator);
        static Func<ViewModelBase, ValueContainer<string>> m_AdvanceGameKeyLocator =
            RegisterContainerLocator<string>(
                "AdvanceGameKey",
                model =>
                    model.m_AdvanceGameKey.Container =
                        model.m_AdvanceGameKey.Container
                        ??
                        new ValueContainer<string>("AdvanceGameKey", model));
        #endregion



        








    }
}
