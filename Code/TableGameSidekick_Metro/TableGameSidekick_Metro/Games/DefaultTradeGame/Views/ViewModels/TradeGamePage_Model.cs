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

using MVVMSidekick.Common;
using MVVMSidekick.Reactive;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Views.SubViews;
using TableGameSidekick_Metro.Games.DefaultTradeGame.Models;
using MVVMSidekick.Storages;
namespace TableGameSidekick_Metro.Games.DefaultTradeGame.Views.ViewModels
{



    public class TradeGamePage_Model : ViewModelBase<TradeGamePage_Model>
    {
        public static Type ExchangeViewType;
        //public static Type GameMainViewType;
        public static Type SetupGameViewType;
        public static Type ScoreBoardViewType;
        public static Type TradeGamePageViewType;

        public TradeGamePage_Model()
        {



        }





        public TradeGamePage_Model(IStorage<TradeGameData> storage, GameInfomation gameInfomation)
        {
            m_GameInfomation = gameInfomation;
            m_Storage = storage;


            if (storage.Value == null)
            {
                m_Storage.Value = GameData = new TradeGameData();

                foreach (var player in gameInfomation.Players)
                {
                    GameData.PlayersData.Add(new PlayerData { PlayerInfomation = player, Resources = new ObservableCollection<ResourcesEntry>() });
                }

            }



            base.ValidateModel =
            ea =>
            {
                SetError(null);
                var sumdic =
                    GameData.PlayersData.SelectMany(x =>
                        {
                            return x.Resources;
                        })
                    .Concat(GameData.BankersStash)
                    .GroupBy(itm =>
                        {
                            return itm.ResourceName;
                        },
                        itm =>
                        {
                            return itm.Amount;
                        })
                    .ToDictionary(g =>
                        {
                            return g.Key;
                        }, g => g.Sum());

                foreach (var item in GameData.ResouceLimitations)
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
                        if (!this.IsUIBusy)
                        {
                            this.IsUIBusy = true;

                            if (!this.GameData.IsStarted)
                            {
                                var data = await Navigator.FrameNavigate<TradeGameData>(
                                    SetupGameViewType,
                                    this
                                    );

                                //if (setupOk)
                                //{
                                //    this.GameData.IsStarted = true;
                                //}
                                storage.Value = this.GameData;
                                await storage.Save();

                            }
                            this.IsUIBusy = false;

                        }

                    }

                );
        }
        GameInfomation m_GameInfomation;

        IStorage<TradeGameData> m_Storage;


        public TradeGameData GameData
        {
            get { return m_GameDataLocator(this).Value; }
            set { m_GameDataLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property TradeGameData GameData Setup
        protected Property<TradeGameData> m_GameData =
          new Property<TradeGameData> { LocatorFunc = m_GameDataLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<TradeGameData>> m_GameDataLocator =
            RegisterContainerLocator<TradeGameData>(
                "GameData",
                model =>
                {
                    model.m_GameData =
                        model.m_GameData
                        ??
                        new Property<TradeGameData> { LocatorFunc = m_GameDataLocator };
                    return model.m_GameData.Container =
                        model.m_GameData.Container
                        ??
                        new ValueContainer<TradeGameData>("GameData", model);
                });
        #endregion
        public CommandModel<ReactiveCommand, String> OnLoadCommand
        {
            get { return m_OnLoadCommand.WithViewModel(this); }
            protected set { m_OnLoadCommand = value; }
        }

        #region OnLoadCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_OnLoadCommand
             = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion


    }
}
