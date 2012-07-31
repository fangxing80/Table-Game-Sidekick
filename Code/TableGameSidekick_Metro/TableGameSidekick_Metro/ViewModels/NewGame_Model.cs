using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using MVVM.Reactive;
namespace TableGameSidekick_Metro.ViewModels
{
    public class NewGame_Model:ViewModelBase<NewGame_Model>
    {

        public NewGame_Model()
        { 
            
        }


        public GameInfomation GameInfomation
        {
            get { return m_GameInfomation.Locate(this).Value; }
            set { m_GameInfomation.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation GameInfomation Setup
        protected Property<GameInfomation> m_GameInfomation = new Property<GameInfomation>(m_GameInfomationLocator);
        static Func<ViewModelBase, ValueContainer<GameInfomation>> m_GameInfomationLocator =
            RegisterContainerLocator<GameInfomation>(
                "GameInfomation",
                model =>
                    model.m_GameInfomation.Container =
                        model.m_GameInfomation.Container
                        ??
                        new ValueContainer<GameInfomation>("GameInfomation",new GameInfomation (), model));
        #endregion




        CommandModel<ReactiveCommand, string> m_StartGameCommand
            = new ReactiveCommand().CreateCommandModel("StartGameCommand");
        public CommandModel<ReactiveCommand, string> StartGameCommand
        {
            get { return m_StartGameCommand.WithViewModel(this); }
            protected set { m_StartGameCommand = value; }
        }
        
        

        
    }
}
