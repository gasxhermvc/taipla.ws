using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Extensions
{
    public static class StreamExtension
    {
        public static byte[] ToArray(this Stream stream)
        {
            byte[] bytesArray;

            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                bytesArray = ms.ToArray();
            }

            return bytesArray;
        }

        public static void GenerateImage(this string fileName, Dictionary<string, dynamic> imageCollection)
        {
            string[] splitName = System.IO.Path.GetFileName(fileName).Split('.');
            var file = System.IO.File.ReadAllBytes(fileName);
            Stream stream = new MemoryStream(file);
            string renameFormat = "{0}@{1}.{2}";

            foreach (var key in imageCollection.Keys)
            {
                string rename = string.Format(renameFormat,
                    splitName[0],
                    key.ToLower(),
                    splitName[1]);

                var imageFormat = System.IO.Path.GetExtension(fileName).GetImageFormat();
                var resize = stream.ResizeImage(imageFormat, (int)imageCollection[key].width, (int)imageCollection[key].height);

                var combine = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(fileName),
                    rename);

                System.IO.File.WriteAllBytes(combine, resize);
            }
        }

        public static byte[] ResizeImage(this Stream stream, ImageFormat format, int width, int height)
        {
            byte[] imageReisze = null;
            Bitmap bitmap = new Bitmap(width, height);

            using (MemoryStream ms = new MemoryStream())
            {
                Image image = Image.FromStream(stream, true, true);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.DrawImage(image, 0, 0, width, height);
                }

                bitmap.Save(ms, format);
                imageReisze = ms.ToArray();

                ms.Close();
                ms.Dispose();
            }

            return imageReisze;
        }

        public static ImageFormat GetImageFormat(this string ext)
        {
            switch (ext.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                default:
                    return ImageFormat.Png;
            }
        }
    }
}
