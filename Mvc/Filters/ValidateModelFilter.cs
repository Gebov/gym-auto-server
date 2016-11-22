using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gym.Mvc.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
                throw new InvalidModelException("Invalid model", actionContext.ModelState);
        }
    }

    internal class InvalidModelException : Exception
    {
        private ModelStateDictionary modelState;
        public InvalidModelException(string message, ModelStateDictionary modelState)
            : base(message)
        {
            this.modelState = modelState;
        }

        public ModelStateDictionary ModelState
        {
            get
            {
                return this.modelState;
            }
        }
    }
}