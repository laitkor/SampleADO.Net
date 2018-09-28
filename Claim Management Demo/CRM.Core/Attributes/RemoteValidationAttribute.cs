using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace CRM.Core.Attributes
{
    public class RemoteValidationAttribute : RemoteAttribute
    {
        public RemoteValidationAttribute(string routeName)
            : base(routeName)
        {
        }

        public RemoteValidationAttribute(string action, string controller)
            : base(action, controller)
        {
        }

        public RemoteValidationAttribute(string action, string controller,
            string areaName)
            : base(action, controller, areaName)
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var controller = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(type => String.Equals(type.Name, string.Format("{0}Controller",
                    this.RouteData["controller"].ToString()), StringComparison.CurrentCultureIgnoreCase));
            if (controller == null) return new ValidationResult(base.ErrorMessageString);
            var action = controller.GetMethods()
                .FirstOrDefault(method => method.Name.ToLower() ==
                                          this.RouteData["action"].ToString().ToLower());
            if (action == null) return new ValidationResult(base.ErrorMessageString);
            var instance = Activator.CreateInstance(controller);
            var response = action.Invoke(instance, new object[] { value });
            if (!(response is JsonResult)) return new ValidationResult(base.ErrorMessageString);
            var jsonData = ((JsonResult)response).Data;
            if (jsonData is bool)
            {
                return (bool)jsonData ? ValidationResult.Success :
                    new ValidationResult(this.ErrorMessage);
            }
            return new ValidationResult(base.ErrorMessageString);
        }


    }
}
