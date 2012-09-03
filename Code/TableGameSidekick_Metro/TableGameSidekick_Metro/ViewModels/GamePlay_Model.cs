using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using System.Runtime.Serialization;
namespace TableGameSidekick_Metro.ViewModels
{
    [DataContract]
    public class GamePlay_Model : ViewModelBase<GamePlay_Model>
    {

        public GamePlay_Model()
        {


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
        static Func<ViewModelBase, ValueContainer<GameInfomation>> m_CurrentGameInfomationLocator =
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



















    }
}
