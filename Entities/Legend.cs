using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class Legend
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public sbyte LegendType { get; set; }
        public string Thumbnail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
