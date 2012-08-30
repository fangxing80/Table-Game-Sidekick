using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TableGameSidekick_Metro.DataEntity
{
    [DataContract]
    public enum GameType
    {
        [EnumMember]
        NotSet = 0,
        [EnumMember]
        TradeGame=1,
        [EnumMember]
        ScoreGame=2,
        [EnumMember]
        StopwatchGame=3,
        [EnumMember]
        Advanced=4
    }
}
