using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModels;
using TableGameSidekick_Metro.DataEntity;
namespace TableGameSidekick_Metro.ViewModels
{
    public class GamePlay_Model : ViewModelBase<GamePlay_Model>
    {

        public GamePlay_Model()
        { 
            
        
        }


        public GameInfomation CurrentGameInfomation
        {
            get { return m_CurrentGameInfomationContainerLocator(this).Value; }
            set { m_CurrentGameInfomationContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property GameInfomation CurrentGameInfomation Setup
        protected PropertyContainer<GameInfomation> m_CurrentGameInfomation;
        protected static Func<object, PropertyContainer<GameInfomation>> m_CurrentGameInfomationContainerLocator =
            RegisterContainerLocator<GameInfomation>(
                "CurrentGameInfomation",
                model =>
                    model.m_CurrentGameInfomation =
                        model.m_CurrentGameInfomation
                        ??
                        new PropertyContainer<GameInfomation>("CurrentGameInfomation"));
        #endregion


        

        




        
    }
}
