using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IO;
using Newtonsoft.Json;

namespace Taipla.Webservice.Filters.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
            IConfiguration configuration,
            ILoggerFactory logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger.CreateLogger("REQ_RES");
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await LogRequest(httpContext);
            var loggingResponse = _configuration.GetSection("Application:Logging").GetValue<bool>("Response");
            if (loggingResponse)
            {
                await LogResponse(httpContext);
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            var loggingRequest = _configuration.GetSection("Application:Logging").GetValue<bool>("Request");

            if (loggingRequest)
            {
                var TRACEID = Activity.Current?.Id ?? context?.TraceIdentifier;

                if (TRACEID.StartsWith("|") && TRACEID.EndsWith("."))
                {
                    TRACEID = TRACEID.Replace("|", string.Empty).Replace(".", string.Empty);
                }

                string IP = context.Connection.RemoteIpAddress.ToString();
                string URL = string.Format("{0}://{1}{2}", context.Request.Scheme, context.Request.Host, context.Request.Path);

                StringValues USER_AGENT = string.Empty;
                context.Request.Headers.TryGetValue("User-Agent", out USER_AGENT);
                string requestMsg = string.Format("LOG: REQUEST|TRACEID: {0}|IP: {1}|URL: {2}|METHOD: {3}|QUERYSTRING: {4}|BODY: {5}|USER AGENT: {6}",
                    TRACEID,
                    IP,
                    URL,
                    context.Request.Method,
                    context.Request.QueryString,
                    "",
                    (!string.IsNullOrEmpty(USER_AGENT.ToString()) ? USER_AGENT.ToString() : "-"));

                _logger.LogInformation(requestMsg);
            }

            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;
            await _next(context);

            var loggingResponse = _configuration.GetSection("Application:Logging").GetValue<bool>("Response");
            var routes = context.Request.RouteValues.ContainsKey("controller");
            if (loggingResponse)
            {
                string responseMsg = string.Empty;

                var TRACEID = Activity.Current?.Id ?? context?.TraceIdentifier;

                if (TRACEID.StartsWith("|") && TRACEID.EndsWith("."))
                {
                    TRACEID = TRACEID.Replace("|", string.Empty).Replace(".", string.Empty);
                }

                if (routes)
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var text = await new StreamReader(context.Response.Body).ReadToEndAsync();

                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);

                    responseMsg = string.Format("LOG: RESPONSE|TRACEID: {0}|STATUSCODE: {1}|RESPONSEBODY: {2}",
                      TRACEID,
                      context.Response.StatusCode,
                      text);

                }
                else
                {
                    responseMsg = string.Format("LOG: RESPONSE|TRACEID: {0}|STATUSCODE: {1}",
                       TRACEID,
                       context.Response.StatusCode);
                }

                _logger.LogInformation(responseMsg);
            }
        }

        private static string ReadStreamInChunks(Stream stream, HttpContext context)
        {
            //const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);

            //if (context.Request.HasFormContentType)
            //{
            //    return JsonConvert.SerializeObject(context.Request.Form);
            //}
            //else
            //{
            //    return string.Empty;
            //}
            return string.Empty;
            //using var textWriter = new StringWriter();
            //using var reader = new StreamReader(stream);
            //var readChunk = new char[readChunkBufferLength];
            //int readChunkLength;
            //do
            //{
            //    readChunkLength = reader.ReadBlock(readChunk,
            //                                       0,
            //                                       readChunkBufferLength);
            //    textWriter.Write(readChunk, 0, readChunkLength);
            //} while (readChunkLength > 0);
            //return textWriter.ToString();
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
