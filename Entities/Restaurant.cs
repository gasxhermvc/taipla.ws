using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class Restaurant
    {
        public int ResId { get; set; }
        public int UserId { get; set; }
        public int? OwnerId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string Map { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Line { get; set; }
        public string Video { get; set; }
        public string OpenTime { get; set; }
        public string Phone { get; set; }
        public string Tags { get; set; }
        public int CarPark { get; set; }
        public string Thumbnail { get; set; }
        public int Viewer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
