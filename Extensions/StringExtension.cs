using System;
using System.Linq;

namespace Taipla.Webservice.Extensions
{
    public static class StringExtension
    {
        public static string Ellipsis(this string str, int len = 30)
        {
            if (str.Length > len)
            {
                return str.Substring(0, len) + "...";
            }
            else
            {
                return str;
            }
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}