using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CRM.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EmailAddressAttribute : DataTypeAttribute, IClientValidatable
    {
        private static Regex _regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        public EmailAddressAttribute()
            : base(DataType.EmailAddress)
        {
            ErrorMessage = "The {0} field is not a valid e-mail address.";
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ValidationType = "email",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string valueAsString = value as string;
            if(valueAsString!=null)
            {
                if( _regex.IsMatch(valueAsString))
                {
                    return true;
                }
            }
           

            return false;
        }
    }
   
   
}
