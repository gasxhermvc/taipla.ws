using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Responses.Backend.RestaurantMenu
{
    public class RestaurantMenuResponse
    {
        public int MENU_ID { get; set; }
        public int RES_ID { get; set; }
        public string COUNTRY_NAME_TH { get; set; }
        public string CULTURE_NAME_TH { get; set; }
        public string NAME_TH { get; set; }
        public string NAME_EN { get; set; }
        public string DESCRIPTION { get; set; }
        public string COOKING_FOOD { get; set; }
        public string DIETETIC_FOOD { get; set; }
        //public string INGREDIENT { get; set; }
        public int COUNTRY_ID { get; set; }
        public int CULTURE_ID { get; set; }
        public string CODE { get; set; }
        public int VIEWER { get; set; }
        public string THUMBNAIL { get; set; }
        public double PRICE { get; set; }
        public int LEGEND_STATUS { get; set; }
        public int? OWNER_ID { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        //=>Displayed
        public List<NzUploadFile> UPLOAD_FILES { get; set; }
    }
}
