﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class FoodCenter
    {
        public int FoodId { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string CookingFood { get; set; }
        public string DieteticFood { get; set; }
        public string Ingredient { get; set; }
        public int CountryId { get; set; }
        public int CultureId { get; set; }
        public string Code { get; set; }
        public int Viewer { get; set; }
        public string Thumbnail { get; set; }
        public int UserId { get; set; }
        public int LegendStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
