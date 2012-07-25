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
        TradeGame=0,
        [EnumMember]
        ScoreGame=1,
        [EnumMember]
        StopwatchGame=2,
        [EnumMember]
        Advanced=3
    }
}
