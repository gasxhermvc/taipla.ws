using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum DeletedEnum
    {
        [Description("ยังไม่ลบ")]
        Delete = 0,
        [Description("ลบแล้ว")]
        Deleted = 1
    }
}
