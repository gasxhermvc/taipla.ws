using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Taipla.Webservice.Extensions
{
    public class DateTimeExtension
    {
        // ตัวแปรสำหรับแปลงค่าเวลา ค.ศ. ไปเป็นปี พ.ศ.
        // for convert 2018 to 2561
        // example string dt = DateTime.Parse("2018-01-01").ToString("dd MMMM yyyy",format_th)
        // output => 01 มกราคม 2561
        public static IFormatProvider format_th = new CultureInfo("th-TH");

        // ตัวแปรสำหรับแปลงค่าเวลา พ.ศ. ไปเป็นปี ค.ศ.
        // for convert 2561 to 2018
        // example string dt = DateTime.Parse("13/01/2561", GlobalFunctions.format_th).ToString("yyyy-MM-dd")
        // output => 2018-13-01
        // or example DateTime dt = DateTime.ParseExact("13/01/2561","dd/MM/yyyy",GlobalFunctions.format_th)
        // output => 2018-01-13 00:00:00
        public static IFormatProvider format_us = new CultureInfo("en-US");

        // ตั้งค่าเวลาเริ่มต้นของแอปพลิเคชัน
        public static void SetDateEnv()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo info = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            info.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            info.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = info;
        }
    }
}
