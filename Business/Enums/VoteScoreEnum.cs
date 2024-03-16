using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum VoteScoreEnum
    {
        [Description("แย่มาก")]
        POOR = 1,

        [Description("พอใช้")]
        FAIR = 2,

        [Description("ดี")]
        GOOD = 3,

        [Description("ดีมาก")]
        VERY_GOOD = 4,

        [Description("เยี่ยมมาก")]
        EXCELLENT = 5

    }
}
