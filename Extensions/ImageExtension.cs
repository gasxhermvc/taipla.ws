using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Entities;

namespace Taipla.Webservice.Extensions
{
    public static class ImageExtension
    {
        public const string DEFAULT_IMAGE = "images/DEFAULTS/default-image/default-image.png";
        public const string DEFAULT_AVATAR = "images/DEFAULTS/default-image/default-avatar.png";
        public static ImageResize GetImage(this IWebHostEnvironment env,
            IHttpContextAccessor context, Media media)
        {
            var imageUrl = !string.IsNullOrEmpty(media.Path) ? string.Format("{0}/{1}",
                                    context.HttpContext.Request.GetUrl(env).Trim(), media.Path) : string.Empty;

            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }


            var ext = System.IO.Path.GetExtension(media.Filename);
            var fileRename = media.Filename.Replace(ext, string.Empty);
            var fileRenameTemp = fileRename + "@{0}" + ext;

            return new ImageResize()
            {
                image = imageUrl,
                imageSM = imageUrl.Replace(media.Filename, string.Format(fileRenameTemp, "sm")),
                imageMD = imageUrl.Replace(media.Filename, string.Format(fileRenameTemp, "md")),
                imageLG = imageUrl.Replace(media.Filename, string.Format(fileRenameTemp, "lg"))
            };
        }

        public static ImageResize GetImageThumbnail(this IWebHostEnvironment env,
            IHttpContextAccessor context, params string[] thumbnails)
        {
            ImageResize image = new ImageResize();

            foreach (var thumb in thumbnails)
            {

                var imageUrl = !string.IsNullOrEmpty(thumb) ? string.Format("{0}/{1}",
                                    context.HttpContext.Request.GetUrl(env).Trim(), thumb) : string.Empty;

                if (string.IsNullOrEmpty(imageUrl))
                {
                    continue;
                }

                var filename = thumb.Split('/').LastOrDefault();

                if (string.IsNullOrEmpty(filename))
                {
                    continue;
                }

                var ext = System.IO.Path.GetExtension(filename);
                var fileRename = filename.Replace(ext, string.Empty);
                var fileRenameTemp = fileRename + "@{0}" + ext;

                image = new ImageResize()
                {
                    image = imageUrl,
                    imageSM = imageUrl.Replace(filename, string.Format(fileRenameTemp, "sm")),
                    imageMD = imageUrl.Replace(filename, string.Format(fileRenameTemp, "md")),
                    imageLG = imageUrl.Replace(filename, string.Format(fileRenameTemp, "lg"))
                };

                break;
            }

            return image;
        }
    }

    public class ImageResize
    {
        public string image { get; set; }

        public string imageSM { get; set; }

        public string imageMD { get; set; }

        public string imageLG { get; set; }
    }
}
