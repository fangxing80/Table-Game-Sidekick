using System;
using System.Collections.Generic;
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
            get { return m_ImageContainerLocator(this).Value; }
            set { m_ImageContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property Byte[] Image Setup
        protected PropertyContainer<Byte[]> m_Image;
        protected static Func<object, PropertyContainer<Byte[]>> m_ImageContainerLocator =
            RegisterContainerLocator<Byte[]>(
                "Image",
                model =>
                    model.m_Image =
                        model.m_Image
                        ??
                        new PropertyContainer<Byte[]>("Image"));
        #endregion


        [DataMember]
        public DateTime StartTime
        {
            get { return m_StartTimeContainerLocator(this).Value; }
            set { m_StartTimeContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property DateTime StartTime Setup
        protected PropertyContainer<DateTime> m_StartTime;
        protected static Func<object, PropertyContainer<DateTime>> m_StartTimeContainerLocator =
            RegisterContainerLocator<DateTime>(
                "StartTime",
                model =>
                    model.m_StartTime =
                        model.m_StartTime
                        ??
                        new PropertyContainer<DateTime>("StartTime"));
        #endregion


        [DataMember]
        public DateTime LastEditTime
        {
            get { return m_LastEditTimeContainerLocator(this).Value; }
            set { m_LastEditTimeContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property DateTime LastEditTime Setup
        protected PropertyContainer<DateTime> m_LastEditTime;
        protected static Func<object, PropertyContainer<DateTime>> m_LastEditTimeContainerLocator =
            RegisterContainerLocator<DateTime>(
                "LastEditTime",
                model =>
                    model.m_LastEditTime =
                        model.m_LastEditTime
                        ??
                        new PropertyContainer<DateTime>("LastEditTime"));
        #endregion



        [DataMember]
        public System.Collections.ObjectModel.ObservableCollection<PlayerInfomation> Players
        {
            get { return m_PlayersContainerLocator(this).Value; }
            set { m_PlayersContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property System.Collections.ObjectModel.ObservableCollection<PlayerInfomation>  Players Setup
        protected PropertyContainer<System.Collections.ObjectModel.ObservableCollection<PlayerInfomation>> m_Players;
        protected static Func<object, PropertyContainer<System.Collections.ObjectModel.ObservableCollection<PlayerInfomation>>> m_PlayersContainerLocator =
            RegisterContainerLocator<System.Collections.ObjectModel.ObservableCollection<PlayerInfomation>>(
                "Players",
                model =>
                    model.m_Players =
                        model.m_Players
                        ??
                        new PropertyContainer<System.Collections.ObjectModel.ObservableCollection<PlayerInfomation>>("Players"));
        #endregion


        [DataMember]
        public Guid Id
        {
            get { return m_IdContainerLocator(this).Value; }
            set { m_IdContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property Guid Id Setup
        protected PropertyContainer<Guid> m_Id;
        protected static Func<object, PropertyContainer<Guid>> m_IdContainerLocator =
            RegisterContainerLocator<Guid>(
                "Id",
                model =>
                    model.m_Id =
                        model.m_Id
                        ??
                        new PropertyContainer<Guid>("Id")           );
        #endregion


        [DataMember]
        public GameType  GameType
        {
            get { return m_GameTypeContainerLocator(this).Value; }
            set { m_GameTypeContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property GameType  GameType Setup
        protected PropertyContainer<GameType > m_GameType;
        protected static Func<object, PropertyContainer<GameType >> m_GameTypeContainerLocator =
            RegisterContainerLocator<GameType >(
                "GameType",
                model =>
                    model.m_GameType =
                        model.m_GameType
                        ??
                        new PropertyContainer<GameType >("GameType"));
        #endregion

        [DataMember]
        public string AdvanceGameKey
        {
            get { return m_AdvanceGameKeyContainerLocator(this).Value; }
            set { m_AdvanceGameKeyContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property string AdvanceGameKey Setup
        protected PropertyContainer<string> m_AdvanceGameKey;
        protected static Func<object, PropertyContainer<string>> m_AdvanceGameKeyContainerLocator =
            RegisterContainerLocator<string>(
                "AdvanceGameKey",
                model =>
                    model.m_AdvanceGameKey =
                        model.m_AdvanceGameKey
                        ??
                        new PropertyContainer<string>("AdvanceGameKey"));
        #endregion



        
    }
}
