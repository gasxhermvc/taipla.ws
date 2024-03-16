using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum LegendEnum
    {
        [Description("ตำนานอาหาร")]
        LEGEND_FOOD = 1,
        [Description("ตำนานอาหารของร้านอาหาร")]
        LEGEND_RESTAURANT = 2,
    }
}
