﻿using MVVMSidekick.ViewModels;
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
        public static Type SetupGameViewType;
        public static Type ScoreBoardViewType;
        public static Type TradeGamePageViewType;

        public TradeGamePage_Model()
        {
            if (IsInDesignMode)
            {
                GameData = new TradeGameData();
                GameData.PlayersData.Add(
                    new PlayerData
                        {
                            PlayerInfomation = new PlayerInfomation() { Name = "aaa" },
                            Resources = new ObservableCollection<ResourcesEntry> { 
                                new ResourcesEntry { ResourceName="Gold" ,Amount =500 },
                                new ResourcesEntry { ResourceName="Silver" ,Amount =501 }
                            
                            }

                        }

                    );
                GameData.PlayersData.Add(
                new PlayerData
                {
                    PlayerInfomation = new PlayerInfomation() { Name = "bbb" },
                    Resources = new ObservableCollection<ResourcesEntry> { 
                                new ResourcesEntry { ResourceName="Gold" ,Amount =502 },
                                new ResourcesEntry { ResourceName="Silver" ,Amount =503 }
                            
                            }
                }

            );

            }



        }





        public TradeGamePage_Model(IStorage<TradeGameData> storage, GameInfomation gameInfomation)
        {
            _GameInfomation = gameInfomation;
            _Storage = storage;


            if (storage.Value == null)
            {
                _Storage.Value = GameData = new TradeGameData();

                foreach (var player in gameInfomation.Players)
                {
                    GameData.PlayersData.Add(new PlayerData { PlayerInfomation = player, Resources = new ObservableCollection<ResourcesEntry>() });
                }

            }
            else
            {

                GameData = storage.Value;
            }



            CreatePropertyChangedObservable()
                .Subscribe(
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

            });

            IsValidationActivated = true;

            CommandOnLoadCommand.CommandCore
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
        GameInfomation _GameInfomation;

        IStorage<TradeGameData> _Storage;


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


        
        public CommandModel<ReactiveCommand, String> CommandOnLoadCommand
        {
            get { return _CommandOnLoadCommandLocator(this).Value; }
            set { _CommandOnLoadCommandLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property CommandModel<ReactiveCommand, String> CommandOnLoadCommand Setup
        protected Property<CommandModel<ReactiveCommand, String>> _CommandOnLoadCommand = new Property<CommandModel<ReactiveCommand, String>> { LocatorFunc = _CommandOnLoadCommandLocator };
        static Func<BindableBase, ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandOnLoadCommandLocator = RegisterContainerLocator<CommandModel<ReactiveCommand, String>>("CommandOnLoadCommand", model => model.Initialize("CommandOnLoadCommand", ref model._CommandOnLoadCommand, ref _CommandOnLoadCommandLocator, _CommandOnLoadCommandDefaultValueFactory));
        static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandOnLoadCommandDefaultValueFactory =
            model =>
            {
                var cmd = new ReactiveCommand(canExecute: true) { ViewModel = model }; //New Command Core
                //cmd.Subscribe (_=>{ } ).RegisterDisposeToViewModel(model); //Config it if needed
                return cmd.CreateCommandModel("OnLoadCommand");
            };
        #endregion



    }
}
