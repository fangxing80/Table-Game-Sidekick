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
            get { return m_NameLocator(this).Value ?? ""; }
            set { m_NameLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property string Name Setup

        protected Property<string> m_Name =
          new Property<string> { LocatorFunc = m_NameLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<ViewModelBase, ValueContainer<string>> m_NameLocator =
            RegisterContainerLocator<string>(
            "Name",
            model =>
            {
                model.m_Name =
                    model.m_Name
                    ??
                    new Property<string> { LocatorFunc = m_NameLocator };
                return model.m_Name.Container =
                    model.m_Name.Container
                    ??
                    new ValueContainer<string>("Name", "", model);
            });

        #endregion



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
        static Func<ViewModelBase, ValueContainer<ImageData>> m_ImageLocator =
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




        public override bool Equals(object obj)
        {
            var vo = obj as PlayerInfomation;
            if (vo == null)
            {
                return false;
            }
            return vo.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }







































    }
}
