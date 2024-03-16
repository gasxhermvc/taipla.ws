using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class UserDevice
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public DateTime Expired { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
