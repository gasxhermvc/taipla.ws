using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Extensions
{
    public static class ValidateExtension
    {
        public const string PHONE_NUMBER_THAI = @"^(?=[0|\+66|66])(([0|\+66|66]|[0-9]){2,10}?).*((((\d){7}|(\d){3}|\-(\d){3})?((\d){4}|\-(\d){4})|(\d){7})?)[+?(\-(\d){3,5})]$";
        public const string PHONE_NUMBER_THAI_ERR_MSG = "phone number format only, example: xxx-xxx-xxx or xx-xxx-xxxx or xxx-xxx-xxxx-xxx or 0xxxxxxxx";

        public static bool isRoleValid(string role)
        {
            switch (role.ToLower())
            {
                case "admin":
                case "post":
                case "post_restaurant":
                case "owner":
                case "staff":
                case "client":
                    return true;
                default:
                    return false;
            }
        }

        public static bool isRoleClient(string role)
        {
            return ValidateExtension.isRoleValid(role) && role.ToLower() == "client";
        }

        public static bool isUploadTypeValid(UploadEnum upload)
        {
            switch (upload)
            {
                case UploadEnum.CLIENT:
                case UploadEnum.UM:
                case UploadEnum.FOOD_CULTURE:
                case UploadEnum.FOOD_CENTER:
                case UploadEnum.RESTAURANT:
                case UploadEnum.LEGEND:
                case UploadEnum.RESTAURANT_MENU:
                case UploadEnum.PROMOTION:
                case UploadEnum.COMMENT:
                    return true;
                default:
                    return false;
            }
        }

        public static bool isDeleteValid(int delete)
        {
            switch (delete)
            {
                case 0:
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        public static string ReplaceWithPhoneNumberThai(string phoneNumber)
        {
            return phoneNumber.Replace("-", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace(" ", string.Empty)
                .Trim();
        }


        public static bool isDirectionValid(int direction)
        {
            switch (direction)
            {
                case -1:
                case 0:
                case 1:
                case 2:
                    return true;
                default:
                    return false;
            }
        }

        public static bool isPromotionTypeValid(int promotionType)
        {
            switch (promotionType)
            {
                case 0:
                case 1:
                case 2:
                    return true;
                default:
                    return false;
            }
        }


        public static bool isDateTimeBetweenIsValid(DateTime? start, DateTime? end)
        {
            if (start != null && end == null)
            {
                return false;
            }

            if (start == null && end != null)
            {
                return false;
            }

            if (end < start)
            {
                return false;
            }

            return true;
        }
    }
}
