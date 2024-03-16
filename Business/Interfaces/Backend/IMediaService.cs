using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Media;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IMediaService
    {
        Task<BaseResponse> Medias(MediaParameter param);

        Task<BaseResponse> Upload(MediaUploadParameter param);

        Task<BaseResponse> RemoveUpload(MediaRemoveUploadParameter param);
    }
}
