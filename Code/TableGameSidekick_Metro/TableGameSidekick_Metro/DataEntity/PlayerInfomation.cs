using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;

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
        public byte[] Image
        {
            get { return m_Image.Locate(this).Value; }
            set { m_Image.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property byte[] Image Setup
        protected Property<byte[]> m_Image = new Property<byte[]>(m_ImageLocator);
        static Func<ViewModelBase, ValueContainer<byte[]>> m_ImageLocator =
            RegisterContainerLocator<byte[]>(
                "Image",
                model =>
                    model.m_Image.Container =
                        model.m_Image.Container
                        ??
                        new ValueContainer<byte[]>("Image", model));
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



        

        


        



        













    }
}
