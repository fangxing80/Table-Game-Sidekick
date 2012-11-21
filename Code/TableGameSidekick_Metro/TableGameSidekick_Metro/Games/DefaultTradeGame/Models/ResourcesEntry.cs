using MVVMSidekick.Reactive;
using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TableGameSidekick_Metro.DataEntity;

namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{

        
        


    [DataContract]
    public class ResourcesEntry : BindableBase<ResourcesEntry>
    {
        [DataMember]        
        public string ResourceName
        {
            get { return _ResourceNameLocator(this).Value; }
            set { _ResourceNameLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property string ResourceName Setup
        protected Property<string> _ResourceName = new Property<string> { LocatorFunc = _ResourceNameLocator };
        static Func<BindableBase, ValueContainer<string>> _ResourceNameLocator = RegisterContainerLocator<string>("ResourceName",
            model => model
                .Initialize("ResourceName", ref model._ResourceName, ref _ResourceNameLocator, _ResourceNameDefaultValueFactory));          
        static Func<string> _ResourceNameDefaultValueFactory = null;
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


        [DataMember]
        public Double Amount
        {
            get { return _AmountLocator(this).Value; }
            set { _AmountLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Double Amount Setup
        protected Property<Double> _Amount =
          new Property<Double> { LocatorFunc = _AmountLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Double>> _AmountLocator =
            RegisterContainerLocator<Double>(
                "Amount",
                model =>
                {
                    model._Amount =
                        model._Amount
                        ??
                        new Property<Double> { LocatorFunc = _AmountLocator };
                    return model._Amount.Container =
                        model._Amount.Container
                        ??
                        new ValueContainer<Double>("Amount", model);
                });
        #endregion

        
        





    }
}
