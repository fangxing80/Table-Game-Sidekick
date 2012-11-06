using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TableGameSidekick_Metro.DataEntity;


namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{

    [DataContract]
    public class ResourceConfig : BindableBase<ResourceConfig>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性

        public ResourceConfig(int players)
        {
            ValidateModel =
                _ =>
                {
                    if (CheckError(() => TotalAmount < 0, "ERROR_TotalAmount_LESS_THAN_ZERO")) return;
                    if (CheckError(() => EachPlayerAmount < 0, "ERROR_EachPlayerAmount_LESS_THAN_ZERO")) return;
                    if (CheckError(() => EachPlayerAmount * Players > TotalAmount, "ERROR_EACH_PLAYER_AMOUNT_OVERFLOW")) return;
                };

        }


        public int Players
        {
            get { return m_PlayersLocator(this).Value; }
            set { m_PlayersLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property int Players Setup
        protected Property<int> m_Players =
          new Property<int> { LocatorFunc = m_PlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<int>> m_PlayersLocator =
            RegisterContainerLocator<int>(
                "Players",
                model =>
                {
                    model.m_Players =
                        model.m_Players
                        ??
                        new Property<int> { LocatorFunc = m_PlayersLocator };
                    return model.m_Players.Container =
                        model.m_Players.Container
                        ??
                        new ValueContainer<int>("Players", model);
                });
        #endregion



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





        public string ImageKey
        {
            get { return m_ImageKeyLocator(this).Value; }
            set { m_ImageKeyLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property string ImageKey Setup
        protected Property<string> m_ImageKey =
          new Property<string> { LocatorFunc = m_ImageKeyLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> m_ImageKeyLocator =
            RegisterContainerLocator<string>(
                "ImageKey",
                model =>
                {
                    model.m_ImageKey =
                        model.m_ImageKey
                        ??
                        new Property<string> { LocatorFunc = m_ImageKeyLocator };
                    return model.m_ImageKey.Container =
                        model.m_ImageKey.Container
                        ??
                        new ValueContainer<string>("ImageKey", model);
                });
        #endregion


        public ImageData Image
        {
            get { return m_ImageLocator(this).Value; }
            set { m_ImageLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ImageData  Image Setup
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



        public Double TotalAmount
        {
            get { return m_TotalAmountLocator(this).Value; }
            set { m_TotalAmountLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Double TotalAmount Setup
        protected Property<Double> m_TotalAmount =
          new Property<Double> { LocatorFunc = m_TotalAmountLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Double>> m_TotalAmountLocator =
            RegisterContainerLocator<Double>(
                "TotalAmount",
                model =>
                {
                    model.m_TotalAmount =
                        model.m_TotalAmount
                        ??
                        new Property<Double> { LocatorFunc = m_TotalAmountLocator };
                    return model.m_TotalAmount.Container =
                        model.m_TotalAmount.Container
                        ??
                        new ValueContainer<Double>("TotalAmount", model);
                });
        #endregion




        public double EachPlayerAmount
        {
            get { return m_EachPlayerAmountLocator(this).Value; }
            set { m_EachPlayerAmountLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property double EachPlayerAmount Setup
        protected Property<double> m_EachPlayerAmount =
          new Property<double> { LocatorFunc = m_EachPlayerAmountLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<double>> m_EachPlayerAmountLocator =
            RegisterContainerLocator<double>(
                "EachPlayerAmount",
                model =>
                {
                    model.m_EachPlayerAmount =
                        model.m_EachPlayerAmount
                        ??
                        new Property<double> { LocatorFunc = m_EachPlayerAmountLocator };
                    return model.m_EachPlayerAmount.Container =
                        model.m_EachPlayerAmount.Container
                        ??
                        new ValueContainer<double>("EachPlayerAmount", model);
                });
        #endregion

    }

}
