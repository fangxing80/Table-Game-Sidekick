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
            if (IsInDesignMode)
            {

            }
            else
            {
                ConfigCommands();
            
            }
            
        }

        protected virtual async Task ConfigCommands()
        {
            #region SaveDataCommand 设置
            GetValueContainer(x => x.CurrentGameInfomation)
                .GetValueChangedObservable()
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
            get { return _CurrentGameInfomationLocator(this).Value; }
            set { _CurrentGameInfomationLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation CurrentGameInfomation Setup
        protected Property<GameInfomation> _CurrentGameInfomation = new Property<GameInfomation> { LocatorFunc = _CurrentGameInfomationLocator };
        static Func<BindableBase, ValueContainer<GameInfomation>> _CurrentGameInfomationLocator = RegisterContainerLocator<GameInfomation>("CurrentGameInfomation", model => model.Initialize("CurrentGameInfomation", ref model._CurrentGameInfomation, ref _CurrentGameInfomationLocator, _CurrentGameInfomationDefaultValueFactory));
        static Func<GameInfomation> _CurrentGameInfomationDefaultValueFactory = null;
        #endregion





        public BindableBase GameModel
        {
            get { return _GameModelLocator(this).Value; }
            set { _GameModelLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property BindableBase GameModel Setup
        protected Property<BindableBase> _GameModel =
          new Property<BindableBase> { LocatorFunc = _GameModelLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<BindableBase>> _GameModelLocator =
            RegisterContainerLocator<BindableBase>(
                "GameModel",
                model =>
                {
                    model._GameModel =
                        model._GameModel
                        ??
                        new Property<BindableBase> { LocatorFunc = _GameModelLocator };
                    return model._GameModel.Container =
                        model._GameModel.Container
                        ??
                        new ValueContainer<BindableBase>("GameModel", model);
                });
        #endregion





        public CommandModel<ReactiveCommand, String> BackToMainCommand
        {
            get { return _BackToMainCommand.WithViewModel(this); }
            protected set { _BackToMainCommand = value; }
        }

        #region BackToMainCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _BackToMainCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion















    }
}
