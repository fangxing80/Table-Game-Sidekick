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
    public class PlayerInfomation : ViewModelBase<PlayerInfomation>
    {

        [DataMember]
        public String Name
        {
            get { return m_NameContainerLocator(this).Value; }
            set { m_NameContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property String Name Setup
        protected PropertyContainer<String> m_Name;
        protected static Func<object, PropertyContainer<String>> m_NameContainerLocator =
            RegisterContainerLocator<String>(
                "Name",
                model =>
                    model.m_Name =
                        model.m_Name
                        ??
                        new PropertyContainer<String>("Name"));
        #endregion


        [DataMember]
        public bool IsContact
        {
            get { return m_IsContactContainerLocator(this).Value; }
            set { m_IsContactContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property bool IsContact Setup
        protected PropertyContainer<bool> m_IsContact;
        protected static Func<object, PropertyContainer<bool>> m_IsContactContainerLocator =
            RegisterContainerLocator<bool>(
                "IsContact",
                model =>
                    model.m_IsContact =
                        model.m_IsContact
                        ??
                        new PropertyContainer<bool>("IsContact"));
        #endregion




        [DataMember]
        public string ConractOrResourceKey
        {
            get { return m_ConractOrResourceKeyContainerLocator(this).Value; }
            set { m_ConractOrResourceKeyContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property string  ConractOrResourceKey Setup
        protected PropertyContainer<string> m_ConractOrResourceKey;
        protected static Func<object, PropertyContainer<string>> m_ConractOrResourceKeyContainerLocator =
            RegisterContainerLocator<string>(
                "ConractOrResourceKey",
                model =>
                    model.m_ConractOrResourceKey =
                        model.m_ConractOrResourceKey
                        ??
                        new PropertyContainer<string>("ConractOrResourceKey"));
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
                        new PropertyContainer<Guid>("Id"));
        #endregion













    }
}
