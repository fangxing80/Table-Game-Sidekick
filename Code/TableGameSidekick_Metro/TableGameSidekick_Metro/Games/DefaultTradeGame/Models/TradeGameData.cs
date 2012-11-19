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
            get { return _BankersStashLocator(this).Value; }
            set { _BankersStashLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<ResourcesEntry> BankersStash Setup
        protected Property<ObservableCollection<ResourcesEntry>> _BankersStash = new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = _BankersStashLocator };
        static Func<BindableBase, ValueContainer<ObservableCollection<ResourcesEntry>>> _BankersStashLocator = RegisterContainerLocator<ObservableCollection<ResourcesEntry>>("BankersStash", model => model.Initialize("BankersStash", ref model._BankersStash, ref _BankersStashLocator, _BankersStashDefaultValueFactory));
        static Func<ObservableCollection<ResourcesEntry>> _BankersStashDefaultValueFactory = ()=>new  ObservableCollection<ResourcesEntry>() ;
        #endregion


        /// <summary>
        /// 各种资源最大数目限制
        /// </summary>
         [DataMember ]
        
        public ObservableCollection<ResourcesEntry> ResouceLimitations
        {
            get { return _ResouceLimitationsLocator(this).Value; }
            set { _ResouceLimitationsLocator(this).SetValueAndTryNotify(value); }
        }
         #region Property ObservableCollection<ResourcesEntry> ResouceLimitations Setup
         protected Property<ObservableCollection<ResourcesEntry>> _ResouceLimitations = new Property<ObservableCollection<ResourcesEntry>> { LocatorFunc = _ResouceLimitationsLocator };
         static Func<BindableBase, ValueContainer<ObservableCollection<ResourcesEntry>>> _ResouceLimitationsLocator = RegisterContainerLocator<ObservableCollection<ResourcesEntry>>("ResouceLimitations", model => model.Initialize("ResouceLimitations", ref model._ResouceLimitations, ref _ResouceLimitationsLocator, _ResouceLimitationsDefaultValueFactory));
         static Func<ObservableCollection<ResourcesEntry>> _ResouceLimitationsDefaultValueFactory = ()=>new  ObservableCollection<ResourcesEntry>();
         #endregion


        /// <summary>
        /// 玩家资源
        /// </summary>
        [DataMember ]
        public ObservableCollection<PlayerData> PlayersData
        {
            get { return _PlayersDataLocator(this).Value; }
            set { _PlayersDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property ObservableCollection<PlayerData> PlayersData Setup
        protected Property<ObservableCollection<PlayerData>> _PlayersData =
          new Property<ObservableCollection<PlayerData>> { LocatorFunc = _PlayersDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerData>>> _PlayersDataLocator =
            RegisterContainerLocator<ObservableCollection<PlayerData>>(
                "PlayersData",
                model =>
                {
                    model._PlayersData =
                        model._PlayersData
                        ??
                        new Property<ObservableCollection<PlayerData>> { LocatorFunc = _PlayersDataLocator };
                    return model._PlayersData.Container =
                        model._PlayersData.Container
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
            get { return _IsStartedLocator(this).Value; }
            set { _IsStartedLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property bool IsStarted Setup
        protected Property<bool> _IsStarted =
          new Property<bool> { LocatorFunc = _IsStartedLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<bool>> _IsStartedLocator =
            RegisterContainerLocator<bool>(
                "IsStarted",
                model =>
                {
                    model._IsStarted =
                        model._IsStarted
                        ??
                        new Property<bool> { LocatorFunc = _IsStartedLocator };
                    return model._IsStarted.Container =
                        model._IsStarted.Container
                        ??
                        new ValueContainer<bool>("IsStarted", model);
                });
        #endregion




    }
	
}
