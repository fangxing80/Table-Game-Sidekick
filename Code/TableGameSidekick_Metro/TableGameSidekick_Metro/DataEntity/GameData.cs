using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModels;

namespace TableGameSidekick_Metro.DataEntity
{
    [DataContract]
    public class GameData : ViewModelBase<GameData>
    {

        [DataMember]
        public Guid Id
        {
            get { return m_Id.Locate(this).Value; }
            set { m_Id.Locate(this).SetValueAndTryNotify(value); }
        }
        #region Property Guid Id Setup
        protected Property<Guid> m_Id = new Property<Guid>(m_IdLocator);
        static Func<ViewModelBase, ValueContainer<Guid>> m_IdLocator =
            RegisterContainerLocator<Guid>(
                "Id",
                model =>
                    model.m_Id.Container =
                        model.m_Id.Container
                        ??
                        new ValueContainer<Guid>("Id", model));
        #endregion



        






        [DataMember]
              public GameInfomation GameInfomation
        {
            get { return m_GameInfomation.Locate(this).Value; }
            set { m_GameInfomation.Locate(this).SetValueAndTryNotify(value); }
        }        
        #region Property GameInfomation GameInfomation Setup        
        protected Property<GameInfomation> m_GameInfomation = new Property<GameInfomation>( m_GameInfomationLocator);
        static Func<ViewModelBase,ValueContainer<GameInfomation>> m_GameInfomationLocator=
            RegisterContainerLocator<GameInfomation>(
                "GameInfomation",
                model =>
                    model.m_GameInfomation.Container = 
                        model.m_GameInfomation.Container
                        ??
                        new ValueContainer<GameInfomation>("GameInfomation",model));          
        #endregion
        
              
        

        
              
        




        




        
    }
}
