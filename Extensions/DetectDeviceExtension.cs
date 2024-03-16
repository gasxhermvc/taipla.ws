using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace Taipla.Webservice.Extensions
{
    public static class DetectDeviceExtension
    {
        public static string GetDevicePlatform(this IDetectionService detect)
        {
            var deviceName = string.Empty;

            switch (detect.Device.Type)
            {
                case Device.Desktop:
                    deviceName = "DESKTOP";
                    break;
                case Device.Mobile:
                    deviceName = "MOBILE";
                    break;
                case Device.Tablet:
                    deviceName = "TABLET";
                    break;
                case Device.Watch:
                    deviceName = "WATCH";
                    break;
                case Device.IoT:
                    deviceName = "IOT";
                    break;
                case Device.Tv:
                    deviceName = "TV";
                    break;
                default:
                    deviceName = "UNKNOW";
                    break;
            }

            return deviceName;
        }

        public static string GetDeviceId(this IDetectionService detect, IHttpContextAccessor context, IAuthenticationService<Cores.ClientInfo> authen, string CLIENT_ID = "")
        {
            var deviceId = string.Format("{0}_{1}_{2}_{3}",
                    context.HttpContext.Request.GetHostName(),
                    detect.GetDevicePlatform(),
                    detect.Browser.Name,
                    authen?.User?.CLIENT_ID ?? CLIENT_ID);

            return deviceId;
        }

        public static string GetDeviceId(this IDetectionService detect, IHttpContextAccessor context, IAuthenticationService<Cores.UserInfo> authen, string CLIENT_ID = "")
        {
            var deviceId = string.Format("{0}_{1}_{2}_{3}",
                    context.HttpContext.Request.GetHostName(),
                    detect.GetDevicePlatform(),
                    detect.Browser.Name,
                    authen?.User?.CLIENT_ID ?? CLIENT_ID);

            return deviceId;
        }
    }
}
