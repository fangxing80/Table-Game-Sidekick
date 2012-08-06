using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;

namespace TableGameSidekick_Metro.DataEntity
{
    [DataContract]
    public class GameData : ViewModelBase<GameData>
    {

        [DataMember]

        public Guid Id
        {
            get { return m_IdLocator(this).Value; }
            set { m_IdLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property Guid Id Setup

        protected Property<Guid> m_Id =
          new Property<Guid> { LocatorFunc = m_IdLocator };
        static Func<ViewModelBase, ValueContainer<Guid>> m_IdLocator =
            RegisterContainerLocator<Guid>(
                "Id",
                model =>
                {
                    model.m_Id =
                        model.m_Id
                        ??
                        new Property<Guid> { LocatorFunc = m_IdLocator };
                    return model.m_Id.Container =
                        model.m_Id.Container
                        ??
                        new ValueContainer<Guid>("Id", model);
                });

        #endregion











        [DataMember]
        
        public GameInfomation GameInfomation
        {
            get { return m_MyPropertyLocator(this).Value; }
            set { m_MyPropertyLocator(this).SetValueAndTryNotify(value); }
        }


        #region Property GameInfomation MyProperty Setup

        protected Property<GameInfomation> m_MyProperty =
          new Property<GameInfomation> { LocatorFunc = m_MyPropertyLocator };
        static Func<ViewModelBase, ValueContainer<GameInfomation>> m_MyPropertyLocator =
            RegisterContainerLocator<GameInfomation>(
                "MyProperty",
                model =>
                {
                    model.m_MyProperty =
                        model.m_MyProperty
                        ??
                        new Property<GameInfomation> { LocatorFunc = m_MyPropertyLocator };
                    return model.m_MyProperty.Container =
                        model.m_MyProperty.Container
                        ??
                        new ValueContainer<GameInfomation>("MyProperty", model);
                });

        #endregion

















    }
}
