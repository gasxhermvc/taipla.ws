using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Models.Bases
{
    public interface IBaseResponse { }

    [Serializable]
    public class BaseResponse : IBaseResponse
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public int total { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    [Serializable]
    public class ErrorResponse : BaseResponse
    {
        public object errors { get; set; }
    }

    [Serializable]
    public class StreamResponse : BaseResponse
    {
        public FileSystemResponse fileStream { get; set; }
    }

    [Serializable]
    public class FileSystemResponse
    {
        public string fileName { get; set; }

        public byte[] rawBytes { get; set; }

        public string mimeType { get; set; }
    }


    public class NzUploadFile
    {
        public string uid { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; }
        public string systemName { get; set; }
        public string status { get; set; } = "done";
        public string url { get; set; }
    }
}
