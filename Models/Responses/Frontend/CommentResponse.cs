using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class CommentResponse
    {
        [JsonProperty("commentId")]
        public int CMT_ID { get; set; }
        [JsonProperty("userId")]
        public int USER_ID { get; set; }
        [JsonProperty("fullName")]
        public string FULL_NAME { get; set; }
        [JsonProperty("comment")]
        public string COMMENT { get; set; }
        [JsonProperty("createDate")]
        public DateTime CREATE_DATE { get; set; }
        [JsonProperty("imageAvatar")]
        public string IMAGE_AVATAR { get; set; }
        [JsonProperty("imageAvatarSM")]
        public string IMAGE_AVATAR_SM { get; set; }
        [JsonProperty("imageAvatarMD")]
        public string IMAGE_AVATAR_MD { get; set; }
        [JsonProperty("imageAvatarLG")]
        public string IMAGE_AVATAR_LG { get; set; }
        [JsonProperty("image")]
        public string IMAGE { get; set; }
        [JsonProperty("imageSM")]
        public string IMAGE_SM { get; set; }
        [JsonProperty("imageMD")]
        public string IMAGE_MD { get; set; }
        [JsonProperty("imageLG")]
        public string IMAGE_LG { get; set; }
    }
}
