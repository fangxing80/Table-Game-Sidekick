using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

using System.Text;

namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Models
{

    [DataContract]
    public class TradeGameData : BindableBase<TradeGameData>
    {
        // If you have install the code sniplets, use "propvm + [tab] +[tab]" create a property。
        // 如果您已经安装了 MVVMSidekick 代码片段，请用 propvm +tab +tab 输入属性

    


        /// <summary>
        /// 庄家资源
        /// </summary>
        [DataMember ]
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


        /// <summary>
        /// 各种资源最大数目限制
        /// </summary>
         [DataMember ]
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
        [DataMember ]
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

        /// <summary>
        /// 游戏是否已经启动
        /// </summary>
        [DataMember ]
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




    }
	
}
