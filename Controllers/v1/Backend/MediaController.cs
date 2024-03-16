using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Media;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private IMediaService _media;
        public MediaController(IMediaService media)
        {
            DateTimeExtension.SetDateEnv();
            _media = media;
        }

        [HttpGet("medias")]
        public async Task<IActionResult> Medias([FromQuery] MediaParameter param)
        {
            var response = await _media.Medias(param);

            return Ok(response);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] MediaUploadParameter param)
        {
            var response = await _media.Upload(param);

            return Ok(response);
        }

        [HttpDelete("remove/upload/{UID}")]
        public async Task<IActionResult> RemoveUpload([FromQuery] MediaRemoveUploadParameter param)
        {
            var response = await _media.RemoveUpload(param);

            return Ok(response);
        }
    }
}
