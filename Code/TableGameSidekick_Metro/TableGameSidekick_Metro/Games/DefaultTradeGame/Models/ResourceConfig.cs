using MVVMSidekick.ViewModels;
using MVVMSidekick.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TableGameSidekick_Metro.DataEntity;
using System.Reactive;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{

    [DataContract]
    public class ResourceConfig : BindableBase<ResourceConfig>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性

        public ResourceConfig(int players)
        {
            CreatePropertyChangedObservable()
                .Subscribe(
                _ =>
                {
                    if (CheckError(() => TotalAmount < 0, "ERROR_TotalAmount_LESS_THAN_ZERO")) return;
                    if (CheckError(() => EachPlayerAmount < 0, "ERROR_EachPlayerAmount_LESS_THAN_ZERO")) return;
                    if (HasLimitition)
                    {
                        if (CheckError(() => EachPlayerAmount * Players > TotalAmount, "ERROR_EACH_PLAYER_AMOUNT_OVERFLOW")) return;
                    }
                }
                );


            var limitObservable = this
                    .GetValueContainer(x => x.HasLimitition)
                    .GetValueChangedObservableWithoutArgs()
                    .Where(_ => this.HasLimitition);
            this.GetValueContainer(X => X.TotalAmount)
                .GetValueChangedObservableWithoutArgs()
                .Merge(limitObservable)

                .Subscribe(
                    x =>
                    {
                        this.MaxPerPlayer = (this.HasLimitition) ? TotalAmount / players : 1000000;
                    }
                )
                .DisposeWith(this);
            this.GetValueContainer(x => x.MaxPerPlayer)
                .GetValueChangedObservableWithoutArgs()
                .Merge(limitObservable)
                .Where(_ =>
                    this.MaxPerPlayer < this.EachPlayerAmount)
                .Subscribe(_ =>
                    this.EachPlayerAmount = MaxPerPlayer)
                .DisposeWith(this);
        }


        public int Players
        {
            get { return _PlayersLocator(this).Value; }
            set { _PlayersLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property int Players Setup
        protected Property<int> _Players =
          new Property<int> { LocatorFunc = _PlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<int>> _PlayersLocator =
            RegisterContainerLocator<int>(
                "Players",
                model =>
                {
                    model._Players =
                        model._Players
                        ??
                        new Property<int> { LocatorFunc = _PlayersLocator };
                    return model._Players.Container =
                        model._Players.Container
                        ??
                        new ValueContainer<int>("Players", model);
                });
        #endregion



        public string ResourceName
        {
            get { return _ResourceNameLocator(this).Value; }
            set { _ResourceNameLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property string ResourceName Setup
        protected Property<string> _ResourceName =
          new Property<string> { LocatorFunc = _ResourceNameLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> _ResourceNameLocator =
            RegisterContainerLocator<string>(
                "ResourceName",
                model =>
                {
                    model._ResourceName =
                        model._ResourceName
                        ??
                        new Property<string> { LocatorFunc = _ResourceNameLocator };
                    return model._ResourceName.Container =
                        model._ResourceName.Container
                        ??
                        new ValueContainer<string>("ResourceName", model);
                });
        #endregion





        public string ImageKey
        {
            get { return _ImageKeyLocator(this).Value; }
            set { _ImageKeyLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property string ImageKey Setup
        protected Property<string> _ImageKey =
          new Property<string> { LocatorFunc = _ImageKeyLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> _ImageKeyLocator =
            RegisterContainerLocator<string>(
                "ImageKey",
                model =>
                {
                    model._ImageKey =
                        model._ImageKey
                        ??
                        new Property<string> { LocatorFunc = _ImageKeyLocator };
                    return model._ImageKey.Container =
                        model._ImageKey.Container
                        ??
                        new ValueContainer<string>("ImageKey", model);
                });
        #endregion


        public ImageData Image
        {
            get { return _ImageLocator(this).Value; }
            set { _ImageLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ImageData  Image Setup
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



        public Double TotalAmount
        {
            get { return _TotalAmountLocator(this).Value; }
            set { _TotalAmountLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property Double TotalAmount Setup
        protected Property<Double> _TotalAmount =
          new Property<Double> { LocatorFunc = _TotalAmountLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Double>> _TotalAmountLocator =
            RegisterContainerLocator<Double>(
                "TotalAmount",
                model =>
                {
                    model._TotalAmount =
                        model._TotalAmount
                        ??
                        new Property<Double> { LocatorFunc = _TotalAmountLocator };
                    return model._TotalAmount.Container =
                        model._TotalAmount.Container
                        ??
                        new ValueContainer<Double>("TotalAmount", model);
                });
        #endregion




        public double EachPlayerAmount
        {
            get { return _EachPlayerAmountLocator(this).Value; }
            set { _EachPlayerAmountLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property double EachPlayerAmount Setup
        protected Property<double> _EachPlayerAmount =
          new Property<double> { LocatorFunc = _EachPlayerAmountLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<double>> _EachPlayerAmountLocator =
            RegisterContainerLocator<double>(
                "EachPlayerAmount",
                model =>
                {
                    model._EachPlayerAmount =
                        model._EachPlayerAmount
                        ??
                        new Property<double> { LocatorFunc = _EachPlayerAmountLocator };
                    return model._EachPlayerAmount.Container =
                        model._EachPlayerAmount.Container
                        ??
                        new ValueContainer<double>("EachPlayerAmount", model);
                });
        #endregion



        public bool HasLimitition
        {
            get { return _HasLimititionLocator(this).Value; }
            set { _HasLimititionLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property bool HasLimitition Setup
        protected Property<bool> _HasLimitition =
          new Property<bool> { LocatorFunc = _HasLimititionLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<bool>> _HasLimititionLocator =
            RegisterContainerLocator<bool>(
                "HasLimitition",
                model =>
                {
                    model._HasLimitition =
                        model._HasLimitition
                        ??
                        new Property<bool> { LocatorFunc = _HasLimititionLocator };
                    return model._HasLimitition.Container =
                        model._HasLimitition.Container
                        ??
                        new ValueContainer<bool>("HasLimitition", model);
                });
        #endregion







        public double MaxPerPlayer
        {
            get { return _MaxPerPlayerLocator(this).Value; }
            set { _MaxPerPlayerLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property double MaxPerPlayer Setup
        protected Property<double> _MaxPerPlayer =
          new Property<double> { LocatorFunc = _MaxPerPlayerLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<double>> _MaxPerPlayerLocator =
            RegisterContainerLocator<double>(
                "MaxPerPlayer",
                model =>
                {
                    model._MaxPerPlayer =
                        model._MaxPerPlayer
                        ??
                        new Property<double> { LocatorFunc = _MaxPerPlayerLocator };
                    return model._MaxPerPlayer.Container =
                        model._MaxPerPlayer.Container
                        ??
                        new ValueContainer<double>("MaxPerPlayer", model);
                });
        #endregion



    }

}
