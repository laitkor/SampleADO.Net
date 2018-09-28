using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace CRM.Core.Attributes
{
    public sealed class DateStartAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            var dateStart = (DateTime)value;
            // Meeting must start in the future time.
            return (dateStart.Date >= DateTime.Now.Date);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "currentdate"
            };
            rule.ValidationParameters["other"] = "*." + DateTime.Now.ToShortDateString();
            yield return rule;
        }
    }

    public sealed class EndDateAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _otherProperty;
        public EndDateAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _otherProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_otherProperty);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "unknown property {0}",
                        _otherProperty
                    )
                );
            }
            var otherValue = (DateTime)property.GetValue(validationContext.ObjectInstance, null);
            var thisValue = (DateTime)value;
            if (thisValue < otherValue)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "greaterthandate"
            };
            rule.ValidationParameters["other"] = "*." + _otherProperty;
            yield return rule;
        }
    }
}
