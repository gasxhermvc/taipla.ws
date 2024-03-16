using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Extensions
{
    public static class StatusCodeExtension
    {
        public static string GetMessage(HttpStatusCode statusCode, ActionEnum action = ActionEnum.SYSTEM)
        {
            int code = (int)statusCode;
            string message = string.Empty;

            switch (code)
            {
                case 200:
                    if (action == ActionEnum.SYSTEM)
                    {
                        message = "สำเร็จ";
                    }
                    else
                    {
                        message = string.Format("{0}สำเร็จ", action.GetString());
                    }
                    break;
                case 201:
                    message = "เพิ่มข้อมูลสำเร็จ";
                    break;
                case 204:
                    message = "ดึงข้อมูลสำเร็จ, แต่ไม่พบข้อมูล";
                    break;
                case 400:
                    if (action == ActionEnum.SYSTEM)
                    {
                        message = "ไม่สำเร็จ, กรุณาตรวจสอบข้อมูลคำร้องขอใหม่อีกครั้ง";
                    }
                    else
                    {
                        message = string.Format("{0}ไม่สำเร็จ, กรุณาตรวจสอบข้อมูลคำร้องขอใหม่อีกครั้ง", action.GetString());
                    }
                    break;
                case 401:
                    message = "ยังไม่ได้ยืนยันตัวตน";
                    break;
                case 403:
                    if (action == ActionEnum.SYSTEM)
                    {
                        message = "ไม่มีสิทธิ์เข้าถึงข้อมูล";
                    }
                    else
                    {
                        message = string.Format("{0}ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล", action.GetString());
                    }
                    break;
                case 404:
                    if (action == ActionEnum.SYSTEM)
                    {
                        message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูล";
                    }
                    else
                    {
                        message = string.Format("{0}ไม่สำเร็จ, เนื่องจากไม่พบข้อมูล", action.GetString());
                    }

                    break;
                case 500:
                    if (action == ActionEnum.SYSTEM)
                    {
                        message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                    }
                    else
                    {
                        message = string.Format("{0}ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ", action.GetString());
                    }
                    break;
            }

            return message;
        }
    }
}
