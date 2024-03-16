using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;

namespace Taipla.Webservice.Cores
{
    public class EFUploadFileUtilities
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public EFUploadFileUtilities(TAIPLA_DbContext dbContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _env = env;
            _configuration = configuration;
        }

        public Media GetMedia(string SystemName, string RefId)
        {
            var media = _dbContext.Media.FirstOrDefault(f =>
                f.SystemName == SystemName &&
                f.RefId == RefId);

            return media;
        }

        public Media GetMedia(string SystemName, string RefId, string Path)
        {
            var media = _dbContext.Media.FirstOrDefault(f =>
                f.SystemName == SystemName &&
                f.RefId == RefId &&
                f.Path == Path);

            return media;
        }

        public List<Media> GetMedias(string SystemName, string RefId, string IgnorePath)
        {
            var medias = _dbContext.Media.Where(w =>
                w.SystemName == SystemName &&
                w.RefId == RefId &&
                w.Path != IgnorePath).ToList();

            return medias;
        }

        public List<Media> GetMedias(string SystemName, List<string> RefId, string IgnorePath)
        {
            var medias = _dbContext.Media.Where(w =>
                w.SystemName == SystemName &&
                RefId.Contains(w.RefId) &&
                w.Path != IgnorePath).ToList();

            return medias;
        }

        public Media DeleteMedia(string pathFile)
        {
            var media = _dbContext.Media.FirstOrDefault(f => f.Path == pathFile);

            return media;
        }

        public bool RemoveFile(Media media)
        {
            List<string> files = this.GetPathFiles(media);

            try
            {
                files.ForEach(file =>
                {
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                });

                return true;
            }
            catch (Exception e)
            {
                return false;

            }
        }

        public bool RemoveFolder(string thumbnail)
        {
            if (string.IsNullOrEmpty(thumbnail))
            {
                return false;
            }

            var pathFile = System.IO.Path.Combine(
                _env.WebRootPath,
                thumbnail);

            if (System.IO.Directory.Exists(
                System.IO.Path.GetDirectoryName(pathFile)))
            {
                System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(pathFile), true);

                return true;
            }

            return false;

        }

        public Media UpdateMedia(Media media, string fileName)
        {
            media.Filename = fileName;
            media.Path = this.GetPhysicalPath(media);

            return media;
        }

        public Media CreateMedia(string SystemName, string fileName, string RefId, DateTime dt)
        {
            var media = new Media
            {
                SystemName = SystemName,
                Filename = fileName,
                RefId = RefId,
                CreatedDate = dt,
                UpdatedDate = dt
            };

            media.Path = this.GetPhysicalPath(media);

            return media;
        }

        public string GetPhysicalPath(Media media)
        {
            string physicalPath = string.Format("images/{0}/{1}/{2}",
                    media.SystemName.ToUpper(),
                    media.RefId,
                    media.Filename);

            return physicalPath;
        }

        public string GetURL(Media media)
        {
            string pathURL = string.Format("images/{0}/{1}/{2}/{3}",
                          media.SystemName.ToUpper(),
                          media.Id,
                          media.RefId,
                          media.Filename);

            return pathURL;
        }

        public string GetPathFile(Media media)
        {
            string pathFile = string.Format("{0}/images/{1}/{2}/{3}",
                     _env.WebRootPath,
                     media.SystemName.ToUpper(),
                     media.RefId,
                     media.Filename);

            return pathFile;
        }

        public List<string> GetPathFiles(Media media)
        {
            List<string> files = new List<string>();

            string pathFile = string.Format("{0}/images/{1}/{2}/{3}",
                    _env.WebRootPath,
                    media.SystemName.ToUpper(),
                    media.RefId,
                    media.Filename);

            files.Add(pathFile);

            List<string> sizes = new List<string>()
            {
                "sm","md","lg"
            };

            string[] splitName = System.IO.Path.GetFileName(pathFile).Split('.');
            string renameFormat = "{0}@{1}.{2}";

            sizes.ForEach(size =>
            {
                string rename = string.Format(renameFormat,
                   splitName[0],
                   size.ToLower(),
                   splitName[1]);

                var combine = System.IO.Path.Combine(
                  System.IO.Path.GetDirectoryName(pathFile),
                  rename);

                files.Add(combine);
            });

            return files;
        }

        public bool SaveAs(Media media, IFormFile upload)
        {
            try
            {
                string pathURL = this.GetURL(media);

                string pathFile = this.GetPathFile(media);

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(pathFile)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(pathFile));
                }

                using (var fileStream = new FileStream(pathFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    upload.CopyTo(fileStream);

                    fileStream.Flush();
                    fileStream.Close();
                }

                var imageConfigCollection = new List<dynamic>();
                var imageConfig = _configuration.GetSection("Application:ImageResize");
                foreach (var config in imageConfig.GetChildren())
                {
                    imageConfigCollection.Add(new
                    {
                        name = config.GetValue<string>("prefix"),
                        width = config.GetValue<int>("width"),
                        height = config.GetValue<int>("height")
                    });
                }

                var collection = imageConfigCollection.ToDictionary(k => k.name as string, v => v);
                pathFile.GenerateImage(collection);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
