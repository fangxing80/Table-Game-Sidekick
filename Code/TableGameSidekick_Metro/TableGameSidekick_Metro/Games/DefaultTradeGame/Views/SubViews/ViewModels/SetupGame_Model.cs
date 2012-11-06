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

namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews.ViewModels
{

    [DataContract]
    public class SetupGame_Model : ViewModelBase<SetupGame_Model>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性
        public SetupGame_Model()
        {
        }

        public SetupGame_Model(TradeGameData_Model gameDataModel)
        {
            GameData = gameDataModel;
        }

        void ConfigCommands()
        {
            AddResourceCommand
                .CommandCore
                .Subscribe
                (
                    _ =>
                    {
                        ResourceConfigs.Add(
                            new ResourceConfig(GameData.PlayersData.Count)
                            {
                                ResourceName = "Resource",
                                ImageKey = "Image",
                            }
                        );

                    }
                );

            RemoveResourceCommand
                .CommandCore
                .Subscribe(
                    _ =>
                    { 
                    
                    
                    }
                ); 

        }


        public ObservableCollection<ResourceConfig> ResourceConfigs
        {
            get { return m_ResourceConfigsLocator(this).Value; }
            set { m_ResourceConfigsLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<ResourceConfig>  ResourceConfigs Setup
        protected Property<ObservableCollection<ResourceConfig>> m_ResourceConfigs =
          new Property<ObservableCollection<ResourceConfig>> { LocatorFunc = m_ResourceConfigsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourceConfig>>> m_ResourceConfigsLocator =
            RegisterContainerLocator<ObservableCollection<ResourceConfig>>(
                "ResourceConfigs",
                model =>
                {
                    model.m_ResourceConfigs =
                        model.m_ResourceConfigs
                        ??
                        new Property<ObservableCollection<ResourceConfig>> { LocatorFunc = m_ResourceConfigsLocator };
                    return model.m_ResourceConfigs.Container =
                        model.m_ResourceConfigs.Container
                        ??
                        new ValueContainer<ObservableCollection<ResourceConfig>>("ResourceConfigs", model, new ObservableCollection<ResourceConfig>());
                });
        #endregion


        public TradeGameData_Model GameData
        {
            get { return m_GameDataLocator(this).Value; }
            protected set { m_GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property TradeGameData_Model GameData Setup
        protected Property<TradeGameData_Model> m_GameData =
          new Property<TradeGameData_Model> { LocatorFunc = m_GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<TradeGameData_Model>> m_GameDataLocator =
            RegisterContainerLocator<TradeGameData_Model>(
                "GameData",
                model =>
                {
                    model.m_GameData =
                        model.m_GameData
                        ??
                        new Property<TradeGameData_Model> { LocatorFunc = m_GameDataLocator };
                    return model.m_GameData.Container =
                        model.m_GameData.Container
                        ??
                        new ValueContainer<TradeGameData_Model>("GameData", model);
                });
        #endregion






        public CommandModel<ReactiveCommand, String> AddResourceCommand
        {
            get { return m_AddResourceCommand.WithViewModel(this); }
            protected set { m_AddResourceCommand = value; }
        }

        #region AddResourceCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_AddResourceCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("AddResourceCommand");
        #endregion






        public CommandModel<ReactiveCommand, String> RemoveResourceCommand
        {
            get { return m_RemoveResourceCommand.WithViewModel(this); }
            protected set { m_RemoveResourceCommand = value; }
        }

        #region RemoveResourceCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_RemoveResourceCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("RemoveResourceCommand");
        #endregion




        public CommandModel<ReactiveCommand, String> StartGameCommmand
        {
            get { return m_StartGameCommmand.WithViewModel(this); }
            protected set { m_StartGameCommmand = value; }
        }

        #region StartGameCommmand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_StartGameCommmand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("StartGameCommmand");
        #endregion



        public CommandModel<ReactiveCommand, String> BackwardCommand
        {
            get { return m_BackwardCommand.WithViewModel(this); }
            protected set { m_BackwardCommand = value; }
        }

        #region BackwardCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_BackwardCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel("BackwardCommand");
        #endregion

        
    }

}



