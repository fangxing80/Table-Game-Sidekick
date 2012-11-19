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
    public class PlayerInfomation : BindableBase<PlayerInfomation>
    {

        [DataMember]

        
        public string Name
        {
            get { return _NameLocator(this).Value; }
            set { _NameLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property string Name Setup
        protected Property<string> _Name = new Property<string> { LocatorFunc = _NameLocator };
        static Func<BindableBase, ValueContainer<string>> _NameLocator = RegisterContainerLocator<string>("Name", model => model.Initialize("Name", ref model._Name, ref _NameLocator, _NameDefaultValueFactory));
        static Func<string> _NameDefaultValueFactory = ()=>"";
        #endregion



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
