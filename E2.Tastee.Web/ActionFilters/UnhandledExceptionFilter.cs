using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using E2.Tastee.Web.Controllers;

namespace E2.Tastee.Web.ActionFilters
{
    public class UnhandledExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null && !context.ExceptionHandled)
            {
                context.Result = BaseController.FailureResult("UNHANDLED EXCEPTION: " + context.Exception.Message);
                context.ExceptionHandled = true;
                Log.Error(context.Exception, "UNHANDLED!");
            }
        }
    }
}
