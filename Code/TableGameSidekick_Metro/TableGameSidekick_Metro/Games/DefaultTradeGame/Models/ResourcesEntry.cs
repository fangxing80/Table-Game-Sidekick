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
            get { return m_ResourceNameLocator(this).Value; }
            set { m_ResourceNameLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property string ResourceName Setup
        protected Property<string> m_ResourceName =
          new Property<string> { LocatorFunc = m_ResourceNameLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> m_ResourceNameLocator =
            RegisterContainerLocator<string>(
                "ResourceName",
                model =>
                {
                    model.m_ResourceName =
                        model.m_ResourceName
                        ??
                        new Property<string> { LocatorFunc = m_ResourceNameLocator };
                    return model.m_ResourceName.Container =
                        model.m_ResourceName.Container
                        ??
                        new ValueContainer<string>("ResourceName", model);
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
        public Double Amount
        {
            get { return m_AmountLocator(this).Value; }
            set { m_AmountLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Double Amount Setup
        protected Property<Double> m_Amount =
          new Property<Double> { LocatorFunc = m_AmountLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Double>> m_AmountLocator =
            RegisterContainerLocator<Double>(
                "Amount",
                model =>
                {
                    model.m_Amount =
                        model.m_Amount
                        ??
                        new Property<Double> { LocatorFunc = m_AmountLocator };
                    return model.m_Amount.Container =
                        model.m_Amount.Container
                        ??
                        new ValueContainer<Double>("Amount", model);
                });
        #endregion



    }
}
