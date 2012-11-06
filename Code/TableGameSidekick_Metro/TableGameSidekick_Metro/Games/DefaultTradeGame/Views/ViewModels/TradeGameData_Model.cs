using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using TableGameSidekick_Metro.DataEntity;
using TableGameSidekick_Metro.ViewModels;
using System.Collections.ObjectModel;
using TableGameSidekick_Metro.Storages;
using MVVMSidekick.Common;
using MVVMSidekick.Reactive;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Models;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Views.ViewModels
{


    [DataContract]
    public class TradeGameData_Model : ViewModelBase<TradeGameData_Model>
    {
        public static Type ExchangeViewType;
        //public static Type GameMainViewType;
        public static Type SetupGameViewType;


        public TradeGameData_Model()
        {



        }

        public TradeGameData_Model(IStorage<TradeGameData_Model> storage, GameInfomation gameInfomation)
        {
            m_GameInfomation = gameInfomation;
            m_Storage = storage;

            foreach (var player in gameInfomation.Players)
            {
                PlayersData.Add(new PlayerData { PlayerInfomation = player, Resources = new ObservableCollection<ResourcesEntry>() });
            }


            base.ValidateModel =
            ea =>
            {
                SetError(null);
                var sumdic =
                    PlayersData.SelectMany(x => x.Resources)
                    .Concat(BankersStash)
                    .GroupBy(itm => itm.ResourceName, itm => itm.Amount)
                    .ToDictionary(g => g.Key, g => g.Sum());

                foreach (var item in ResouceLimitations)
                {
                    double actualSum;
                    if (sumdic.TryGetValue(item.ResourceName, out actualSum))
                    {
                        if (Math.Abs(actualSum - item.Amount) > 1)
                        {
                            SetError("Resource " + item.ResourceName + " overflowed the limitation");
                        }
                        return;
                    }
                    else
                    {
                        SetError("Resource " + item.ResourceName + " not prepared to anyone");
                    }
                }

            };

            OnLoadCommand.CommandCore
                .Subscribe
                (
                    async e =>
                    {
                        if (!this.IsStarted)
                        {
                            var setupOk = await Navigator.FrameNavigate<bool>(
                                SetupGameViewType,
                                this
                                );

                            if (setupOk)
                            {
                                this.IsStarted = true;                                
                            }
                            await storage.Save();

                        }

                    }

                );
        }
        GameInfomation m_GameInfomation;

        IStorage<TradeGameData_Model> m_Storage;


        /// <summary>
        /// 庄家资源
        /// </summary>
        public ObservableCollection<ResourcesEntry> BankersStash
        {
            get { return m_BankersStashLocator(this).Value; }
            set { m_BankersStashLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<ResourcesEntry>  BankersStash Setup
        protected Property<ObservableCollection<ResourcesEntry>> m_BankersStash =
          new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = m_BankersStashLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourcesEntry>>> m_BankersStashLocator =
            RegisterContainerLocator<ObservableCollection<ResourcesEntry>>(
                "BankersStash",
                model =>
                {
                    model.m_BankersStash =
                        model.m_BankersStash
                        ??
                        new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = m_BankersStashLocator };
                    return model.m_BankersStash.Container =
                        model.m_BankersStash.Container
                        ??
                        new ValueContainer<ObservableCollection<ResourcesEntry>>("BankersStash", model, new ObservableCollection<ResourcesEntry>());
                });
        #endregion



        public ObservableCollection<ResourcesEntry> ResouceLimitations
        {
            get { return m_ResouceLimitationsLocator(this).Value; }
            set { m_ResouceLimitationsLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection <ResourcesEntry > ResouceLimitations Setup
        protected Property<ObservableCollection<ResourcesEntry>> m_ResouceLimitations =
          new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = m_ResouceLimitationsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourcesEntry>>> m_ResouceLimitationsLocator =
            RegisterContainerLocator<ObservableCollection<ResourcesEntry>>(
                "ResouceLimitations",
                model =>
                {
                    model.m_ResouceLimitations =
                        model.m_ResouceLimitations
                        ??
                        new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = m_ResouceLimitationsLocator };
                    return model.m_ResouceLimitations.Container =
                        model.m_ResouceLimitations.Container
                        ??
                        new ValueContainer<ObservableCollection<ResourcesEntry>>("ResouceLimitations", model, new ObservableCollection<ResourcesEntry>());
                });
        #endregion


        /// <summary>
        /// 玩家资源
        /// </summary>
        public ObservableCollection<PlayerData> PlayersData
        {
            get { return m_PlayersDataLocator(this).Value; }
            set { m_PlayersDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<PlayerData> PlayersData Setup
        protected Property<ObservableCollection<PlayerData>> m_PlayersData =
          new Property<ObservableCollection<PlayerData>> { LocatorFunc = m_PlayersDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerData>>> m_PlayersDataLocator =
            RegisterContainerLocator<ObservableCollection<PlayerData>>(
                "PlayersData",
                model =>
                {
                    model.m_PlayersData =
                        model.m_PlayersData
                        ??
                        new Property<ObservableCollection<PlayerData>> { LocatorFunc = m_PlayersDataLocator };
                    return model.m_PlayersData.Container =
                        model.m_PlayersData.Container
                        ??
                        new ValueContainer<ObservableCollection<PlayerData>>("PlayersData", model, new ObservableCollection<PlayerData>());
                });
        #endregion



        public bool IsStarted
        {
            get { return m_IsStartedLocator(this).Value; }
            set { m_IsStartedLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property bool IsStarted Setup
        protected Property<bool> m_IsStarted =
          new Property<bool> { LocatorFunc = m_IsStartedLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<bool>> m_IsStartedLocator =
            RegisterContainerLocator<bool>(
                "IsStarted",
                model =>
                {
                    model.m_IsStarted =
                        model.m_IsStarted
                        ??
                        new Property<bool> { LocatorFunc = m_IsStartedLocator };
                    return model.m_IsStarted.Container =
                        model.m_IsStarted.Container
                        ??
                        new ValueContainer<bool>("IsStarted", model);
                });
        #endregion




        public  CommandModel<ReactiveCommand, String> OnLoadCommand
        {
            get { return m_OnLoadCommand.WithViewModel(null); }
            protected set { m_OnLoadCommand = value; }
        }

        #region OnLoadCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
         CommandModel<ReactiveCommand, String> m_OnLoadCommand
              = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion


    }
}
