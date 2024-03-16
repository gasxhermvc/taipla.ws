using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Business.Enums
{
    public enum RoleEnum
    {
        [Description("super_admin")]
        SUPER_ADMIN = 99,
        [Description("admin")]
        ADMIN = 1,
        [Description("post")]
        POST = 2,
        [Description("post_restaurant")]
        POST_RESTAURANT = 3,
        [Description("owner")]
        OWNER = 4,
        [Description("staff")]
        STAFF = 5,
        [Description("client")]
        CLIENT = 6,
        [Description("unknow")]
        UNKNOW = 9
    }
}
