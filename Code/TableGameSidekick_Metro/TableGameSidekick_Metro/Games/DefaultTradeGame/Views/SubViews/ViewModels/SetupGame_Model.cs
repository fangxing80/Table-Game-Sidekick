using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.ViewModels;
using MVVMSidekick.Reactive;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Models;
using System.Reactive.Linq;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews.ViewModels
{

    [DataContract]
    public class SetupGame_Model : ViewModelBase<SetupGame_Model>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性
        public SetupGame_Model()
        {
            ResourceConfigs.Add(new ResourceConfig(4) { EachPlayerAmount = 15, ResourceName = "Gold", TotalAmount = 1000 });
            //ResourceConfigs.Add(new ResourceConfig(4) { EachPlayerAmount = 15, ResourceName = "Copper", TotalAmount = 1000 });

        }

        public SetupGame_Model(TradeGameData gameDataModel)
        {
            GameData = gameDataModel;
            ConfigProperties();
            ConfigCommands();

            //结算~
            this.AddDisposeAction(

                () =>
                {
                    gameDataModel.ResouceLimitations =
                        new ObservableCollection<ResourcesEntry>(
                            this.ResourceConfigs
                            .Select(
                              rc =>
                              {
                                  return new ResourcesEntry { Amount = rc.HasLimitition ? double.MaxValue : rc.TotalAmount, ResourceName = rc.ResourceName };
                              }));


                    foreach (var item in gameDataModel.PlayersData)
                    {
                        item.Resources =
                            new ObservableCollection<ResourcesEntry>(
                                ResourceConfigs
                                    .Select(rc =>
                                    {
                                        return new ResourcesEntry { Amount = rc.EachPlayerAmount, ResourceName = rc.ResourceName };
                                    })
                            );
                    }

                    gameDataModel.BankersStash =
                        new ObservableCollection<ResourcesEntry>(
                                ResourceConfigs
                                    .Select(rc =>
                                    {
                                        return new ResourcesEntry { Amount = rc.HasLimitition ? (rc.TotalAmount - rc.EachPlayerAmount * rc.Players) : (double.MaxValue), ResourceName = rc.ResourceName };
                                    })
                            );




                }


            );

        }
        void ConfigProperties()
        {


        }
        void ConfigCommands()
        {
            //点击创建记录按钮，增加一条记录
            AddResourceCommand
                .CommandCore
                .Subscribe
                (
                    _ =>
                    {
                        ResourceConfigs.Add(
                            new ResourceConfig(GameData.PlayersData.Count)
                            {
                                ResourceName = "Resource" + ResourceConfigs.Count.ToString(),
                                ImageKey = "Image",
                                HasLimitition = false,
                                TotalAmount = 0,
                                MaxPerPlayer = 1000000,
                                EachPlayerAmount =100000,


                            }
                        );

                    }
                ).RegisterDisposeToViewModel(this);
            //点击创建记录按钮，增加一条记录

            CurrentSelectedResourceConfig.GetValueContainer(x => x.Item2)
                .GetValueChangeObservable()
                .Select(
                    x =>
                        x.EventArgs != null
                )
                .Subscribe(
                    RemoveResourceCommand.CommandCore.CanExecuteObserver
                )
                .RegisterDisposeToViewModel(this);
            RemoveResourceCommand
                .CommandCore
                .Subscribe(
                    _ =>
                    {
                        if (CurrentSelectedResourceConfig.Item2 != null && CurrentSelectedResourceConfig.Item1 != -1)
                        {
                            ResourceConfigs.RemoveAt(CurrentSelectedResourceConfig.Item1);
                        }

                    }
            ).RegisterDisposeToViewModel(this);

            BackwardCommand
                .CommandCore
                .Subscribe(
                    _ =>
                    {

                        //this.Navigator.GoBack();

                        this.Close();

                        //离开页面
                    }
   ).RegisterDisposeToViewModel(this);


            this.StartGameCommmand
                .CommandCore
                .Subscribe(
                 async ve =>
                 {
                     if (!IsUIBusy)
                     {
                         IsUIBusy = true;

                         if (this.ResourceConfigs.All(x => (x as IBindableBase).Error == null))
                         {
                             //确定输入合法则开始游戏
                             GameData.IsStarted = true;
                         }
                         else
                         {
                             var d = new Windows.UI.Popups.MessageDialog("Input Inlegal");
                             await d.ShowAsync();


                         }
                         IsUIBusy = false;
                     }

                     //离开页面
                     this.Close();
                 }
                 ).RegisterDisposeToViewModel(this);
        }


        public ObservableCollection<ResourceConfig> ResourceConfigs
        {
            get { return _ResourceConfigsLocator(this).Value; }
            set { _ResourceConfigsLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<ResourceConfig>  ResourceConfigs Setup
        protected Property<ObservableCollection<ResourceConfig>> _ResourceConfigs =
          new Property<ObservableCollection<ResourceConfig>> { LocatorFunc = _ResourceConfigsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourceConfig>>> _ResourceConfigsLocator =
            RegisterContainerLocator<ObservableCollection<ResourceConfig>>(
                "ResourceConfigs",
                model =>
                {
                    model._ResourceConfigs =
                        model._ResourceConfigs
                        ??
                        new Property<ObservableCollection<ResourceConfig>> { LocatorFunc = _ResourceConfigsLocator };
                    return model._ResourceConfigs.Container =
                        model._ResourceConfigs.Container
                        ??
                        new ValueContainer<ObservableCollection<ResourceConfig>>("ResourceConfigs", model, new ObservableCollection<ResourceConfig>());
                });
        #endregion




        public BindableTuple<int, ResourceConfig> CurrentSelectedResourceConfig
        {
            get { return _CurrentSelectedResourceConfigLocator(this).Value; }
            set { _CurrentSelectedResourceConfigLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property BindableTuple<int,ResourceConfig>  CurrentSelectedResourceConfig Setup
        protected Property<BindableTuple<int, ResourceConfig>> _CurrentSelectedResourceConfig =
          new Property<BindableTuple<int, ResourceConfig>> { LocatorFunc = _CurrentSelectedResourceConfigLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<BindableTuple<int, ResourceConfig>>> _CurrentSelectedResourceConfigLocator =
            RegisterContainerLocator<BindableTuple<int, ResourceConfig>>(
                "CurrentSelectedResourceConfig",
                model =>
                {
                    model._CurrentSelectedResourceConfig =
                        model._CurrentSelectedResourceConfig
                        ??
                        new Property<BindableTuple<int, ResourceConfig>> { LocatorFunc = _CurrentSelectedResourceConfigLocator };
                    return model._CurrentSelectedResourceConfig.Container =
                        model._CurrentSelectedResourceConfig.Container
                        ??
                        new ValueContainer<BindableTuple<int, ResourceConfig>>("CurrentSelectedResourceConfig", model, new BindableTuple<int, ResourceConfig>(-1, null));
                });
        #endregion


        public TradeGameData GameData
        {
            get { return _GameDataLocator(this).Value; }
            set { _GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property TradeGameData GameData Setup
        protected Property<TradeGameData> _GameData =
          new Property<TradeGameData> { LocatorFunc = _GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<TradeGameData>> _GameDataLocator =
            RegisterContainerLocator<TradeGameData>(
                "GameData",
                model =>
                {
                    model._GameData =
                        model._GameData
                        ??
                        new Property<TradeGameData> { LocatorFunc = _GameDataLocator };
                    return model._GameData.Container =
                        model._GameData.Container
                        ??
                        new ValueContainer<TradeGameData>("GameData", model);
                });
        #endregion







        public CommandModel<ReactiveCommand, String> AddResourceCommand
        {
            get { return _AddResourceCommand.WithViewModel(this); }
            protected set { _AddResourceCommand = value; }
        }

        #region AddResourceCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _AddResourceCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("AddResourceCommand");
        #endregion






        public CommandModel<ReactiveCommand, String> RemoveResourceCommand
        {
            get { return _RemoveResourceCommand.WithViewModel(this); }
            protected set { _RemoveResourceCommand = value; }
        }

        #region RemoveResourceCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _RemoveResourceCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("RemoveResourceCommand");
        #endregion




        public CommandModel<ReactiveCommand, String> StartGameCommmand
        {
            get { return _StartGameCommmand.WithViewModel(this); }
            protected set { _StartGameCommmand = value; }
        }

        #region StartGameCommmand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _StartGameCommmand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("StartGameCommmand");
        #endregion



        public CommandModel<ReactiveCommand, String> BackwardCommand
        {
            get { return _BackwardCommand.WithViewModel(this); }
            protected set { _BackwardCommand = value; }
        }

        #region BackwardCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _BackwardCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("BackwardCommand");
        #endregion


    }

}



