using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum UploadEnum
    {
        //=>Frontend
        [Description("UM")] //=>Folder name
        CLIENT = 0,



        //=>Backend
        [Description("UM")]
        UM = 1,
        [Description("FOOD_COUNTRY")]
        FOOD_COUNTRY = 2,
        [Description("FOOD_CULTURE")]
        FOOD_CULTURE = 3,
        [Description("FOOD_CENTER")]
        FOOD_CENTER = 4,
        [Description("RESTAURANT")]
        RESTAURANT = 5,
        [Description("LEGEND")]
        LEGEND = 6,
        [Description("RESTAURANT_MENU")]
        RESTAURANT_MENU = 7,
        [Description("PROMOTION")]
        PROMOTION = 8,


        [Description("COMMENT")]
        COMMENT = 9,

        [Description("UNKNOW")]
        UNKNOW = 99
    }
}
