using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using System.Runtime.Serialization;
using MVVMSidekick.Reactive;
using MVVMSidekick.Commands;
using System.Reactive;
using System.Reactive.Linq;
using MVVMSidekick.Storages;
namespace TableGameSidekick_Metro.ViewModels
{

    public class GamePlay_Model : ViewModelBase<GamePlay_Model>
    {



        public GamePlay_Model()
        {

            ConfigCommands();
        }

        protected virtual async Task ConfigCommands()
        {
            #region SaveDataCommand 设置
            GetValueContainer(x => x.CurrentGameInfomation)
                .GetValueChangeObservable()
                .Select(x => x.EventArgs != null)
                .Subscribe(BackToMainCommand.CommandCore.CanExecuteObserver);

            BackToMainCommand.CommandCore.Subscribe
                (
                    e =>
                    {
                        this.Close();
                    }
                );


            #endregion


            await Task.Yield();
        }


        public Guid Id
        {
            get
            {
                return CurrentGameInfomation == null ? new Guid() : CurrentGameInfomation.Id;
            }
        }


        public GameInfomation CurrentGameInfomation
        {
            get { return m_CurrentGameInfomationLocator(this).Value; }
            set { m_CurrentGameInfomationLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameInfomation CurrentGameInfomation Setup

        protected Property<GameInfomation> m_CurrentGameInfomation =
          new Property<GameInfomation> { LocatorFunc = m_CurrentGameInfomationLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<GameInfomation>> m_CurrentGameInfomationLocator =
            RegisterContainerLocator<GameInfomation>(
            "CurrentGameInfomation",
            model =>
            {
                model.m_CurrentGameInfomation =
                    model.m_CurrentGameInfomation
                    ??
                    new Property<GameInfomation> { LocatorFunc = m_CurrentGameInfomationLocator };
                return model.m_CurrentGameInfomation.Container =
                    model.m_CurrentGameInfomation.Container
                    ??
                    new ValueContainer<GameInfomation>("CurrentGameInfomation", model);
            });

        #endregion





        public BindableBase GameModel
        {
            get { return m_GameModelLocator(this).Value; }
            set { m_GameModelLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property BindableBase GameModel Setup
        protected Property<BindableBase> m_GameModel =
          new Property<BindableBase> { LocatorFunc = m_GameModelLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<BindableBase>> m_GameModelLocator =
            RegisterContainerLocator<BindableBase>(
                "GameModel",
                model =>
                {
                    model.m_GameModel =
                        model.m_GameModel
                        ??
                        new Property<BindableBase> { LocatorFunc = m_GameModelLocator };
                    return model.m_GameModel.Container =
                        model.m_GameModel.Container
                        ??
                        new ValueContainer<BindableBase>("GameModel", model);
                });
        #endregion





        public CommandModel<ReactiveCommand, String> BackToMainCommand
        {
            get { return m_BackToMainCommand.WithViewModel(this); }
            protected set { m_BackToMainCommand = value; }
        }

        #region BackToMainCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_BackToMainCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion















    }
}
