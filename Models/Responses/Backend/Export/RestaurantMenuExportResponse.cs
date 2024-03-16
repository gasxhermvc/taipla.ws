using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Backend.Export
{
    public class RestaurantMenuExportResponse
    {
        public int MenuId { get; set; }
        public int ResId { get; set; }
        public string ResName { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string CookingFood { get; set; }
        public string DieteticFood { get; set; }
        public string Ingredient { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int CultureId { get; set; }
        public string CultureName { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public int Viewer { get; set; }
        public string Thumbnail { get; set; }
        public int LegendStatus { get; set; }
        public string LegendDetail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
