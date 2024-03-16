using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class Comment
    {
        public int CmtId { get; set; }
        public int UserId { get; set; }
        public string RefId { get; set; }
        public string SystemName { get; set; }
        public string Comment1 { get; set; }
        public int ImageStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
