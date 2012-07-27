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
        public string Name
        {
            get { return m_Name.Locate(this).Value; }
            set { m_Name.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property string Name Setup
        protected Property<string> m_Name = new Property<string>(m_NameLocator);
        static Func<ViewModelBase, ValueContainer<string>> m_NameLocator =
            RegisterContainerLocator<string>(
                "Name",
                model =>
                    model.m_Name.Container =
                        model.m_Name.Container
                        ??
                        new ValueContainer<string>("Name", model));
        #endregion



        



        


        [DataMember]
        public bool IsContact
        {
            get { return m_IsContact.Locate(this).Value; }
            set { m_IsContact.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property bool IsContact Setup
        protected Property<bool> m_IsContact = new Property<bool>(m_IsContactLocator);
        static Func<ViewModelBase, ValueContainer<bool>> m_IsContactLocator =
            RegisterContainerLocator<bool>(
                "IsContact",
                model =>
                    model.m_IsContact.Container =
                        model.m_IsContact.Container
                        ??
                        new ValueContainer<bool>("IsContact", model));
        #endregion



        



        




        [DataMember]
        public string ContactKeyOrResourceKey
        {
            get { return m_ContactKeyOrResourceKey.Locate(this).Value; }
            set { m_ContactKeyOrResourceKey.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property string ContactKeyOrResourceKey Setup
        protected Property<string> m_ContactKeyOrResourceKey = new Property<string>(m_ContactKeyOrResourceKeyLocator);
        static Func<ViewModelBase, ValueContainer<string>> m_ContactKeyOrResourceKeyLocator =
            RegisterContainerLocator<string>(
                "ContactKeyOrResourceKey",
                model =>
                    model.m_ContactKeyOrResourceKey.Container =
                        model.m_ContactKeyOrResourceKey.Container
                        ??
                        new ValueContainer<string>("ContactKeyOrResourceKey", model));
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



        



        













    }
}
