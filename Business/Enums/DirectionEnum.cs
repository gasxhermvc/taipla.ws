using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum DirectionEnum
    {
        [Description("ทั้งหมด")]
        ALL = -1,
        [Description("ทั้งหมด")]
        ALL_ = 0,

        [Description("อาหารส่วนกลาง")]
        FOOD_CENTER = 1,

        [Description("ร้านอาหาร")]
        RESTAURANT = 2
    }
}
