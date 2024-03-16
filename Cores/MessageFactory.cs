using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Extensions;

namespace Taipla.Webservice.Cores
{
    public class MessageFactory
    {

        private readonly IWebHostEnvironment _web;

        private JObject json = null;

        public MessageFactory(IWebHostEnvironment web)
        {
            _web = web;
            var jsonString = this.ReadJson();
            if (json == null) this.json = JObject.Parse(jsonString);
        }

        private string ReadJson()
        {
            string jsonPath = System.IO.Path.Combine(PathExtension.BasePath(_web), "Business", "Jsons", "Message.json");

            string jsonString = System.IO.File.ReadAllText(jsonPath, System.Text.Encoding.UTF8);

            return jsonString;
        }

        public void Reload()
        {
            var jsonString = this.ReadJson();
            this.json = JObject.Parse(jsonString);
        }

        public string GetMessage(string jsonToken)
        {
            try
            {
                return this.json.SelectToken(this.formatTokenPath(jsonToken)).Value<string>();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetMessage(string targetMessage, string jsonToken)
        {
            string jsonTokenConcat = string.Concat(this.formatTokenPath(targetMessage), ".", this.formatTokenPath(jsonToken));

            try
            {
                return this.json.SelectToken(this.formatTokenPath(jsonTokenConcat)).Value<string>();
            }
            catch
            {
                return string.Empty;
            }
        }

        private string formatTokenPath(string jsonToken)
        {
            return jsonToken.Replace(":", ".")
                .Replace("/", ".")
                .Trim()
                .Trim(':')
                .Trim('/')
                .Trim('.');
        }
    }
}
