using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ValueFilterAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //try to modify text
            try
            {
                if (value != null && this.IsInteger(value))
                {
                    validationContext
                       .ObjectType
                       .GetProperty(validationContext.MemberName)
                       .SetValue(validationContext.ObjectInstance, this.converter(value), null);
                }
            }
            catch (System.Exception)
            {
            }

            //return null to make sure this attribute never say iam invalid
            return null;
        }

        private object converter(object value)
        {
            dynamic number = Convert.ChangeType(value, value.GetType());

            if (number <= -1)
            {
                number = null;
            }

            value = number;

            return value;
        }

        private bool IsInteger(object value)
        {
            var typeName = value.GetType().Name?.ToLower() ?? "unknow";

            switch (typeName)
            {
                case "int":
                case "int32":
                case "int64":
                case "int128":
                case "int256":
                case "float":
                case "single":
                case "decimal":
                case "double":
                    return true;
            }

            return false;
        }
    }
}
