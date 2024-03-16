using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Backend.Export
{
    public class RestaurantExportResponse
    {
        public int ResId { get; set; }
        public int UserId { get; set; }
        public string UserCreate { get; set; }
        public int? OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string Map { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Line { get; set; }
        public string OpenTime { get; set; }
        public string Phone { get; set; }
        public string Thumbnail { get; set; }
        public string IsCarPark { get; set; }
        public int Viewer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
