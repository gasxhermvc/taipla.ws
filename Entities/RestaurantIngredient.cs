using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class RestaurantIngredient
    {
        public int Id { get; set; }
        public int ResId { get; set; }
        public int MenuId { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Weight { get; set; }
        public string Thumbnail { get; set; }
        public string Code { get; set; }
        public int LegendStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
