using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum ActionEnum
    {
        [Description("ข้อความระบบ")]
        SYSTEM = 0,
        [Description("ดึงข้อมูล")]
        QUERY = 1,
        [Description("เพิ่มข้อมูล")]
        INSERT = 2,
        [Description("แก้ไขข้อมูล")]
        UPDATE = 3,
        [Description("ลบช้อมูล")]
        DELETE = 4
    }
}
