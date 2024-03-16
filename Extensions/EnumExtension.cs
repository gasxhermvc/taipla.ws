using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Taipla.Webservice.Extensions
{
    public static class EnumExtension
    {
        public static string GetString<T>(this T _enum) where T : struct
        {
            DescriptionAttribute[]
            attributes = (DescriptionAttribute[])_enum
               .GetType()
               .GetField(_enum.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static T ParseEnum<T>(this string stringValue, T defaultValue)
        {
            // throw an exception if T is not an Enum
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be of Enum type", "T");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name for the enum
            MemberInfo[] fields = type.GetFields();

            foreach (var field in fields)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == stringValue)
                {
                    return (T)Enum.Parse(typeof(T), field.Name);
                }
            }

            //In case we couldn't find a matching description attribute, we'll just return the defaultValue that we provided
            return defaultValue;
        }
    }
}
