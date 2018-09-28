using System.Web.Mvc;

namespace CRM.Core.Attributes
{
    public class ModelClientValidationNumericGreaterThanRule : ModelClientValidationRule
    {
        public ModelClientValidationNumericGreaterThanRule(string errorMessage, object other, bool allowEquality)
        {
            ErrorMessage = errorMessage;
            ValidationType = "numericgreaterthan";
            ValidationParameters["other"] = other;
            ValidationParameters["allowequality"] = allowEquality;
        }
    }
}