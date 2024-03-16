using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum PromotionEnum
    {
        [Description("ซ่อน")]
        HIDE = 0,
        [Description("ใช้วันที่เริ่มต้น-สิ้นสุด")]
        USE_ESTIMATE = 1,
        [Description("แสดงตลอดจนกว่าจะนำออก")]
        FREEZE = 2,
    }
}
