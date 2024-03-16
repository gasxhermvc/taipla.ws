using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class HistorySearch
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string DeviceId { get; set; }
        public string SearchText { get; set; }
        public string Condition { get; set; }
        public int Deleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
