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
            get { return m_IdContainerLocator(this).Value; }
            set { m_IdContainerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property Guid Id Setup
        protected PropertyContainer<Guid> m_Id;
        protected static Func<object, PropertyContainer<Guid>> m_IdContainerLocator =
            RegisterContainerLocator<Guid>(
                "Id",
                model =>
                    model.m_Id =
                        model.m_Id
                        ??
                        new PropertyContainer<Guid>("Id"));
        #endregion

    }
}
