using Microsoft.AspNetCore.Mvc;

namespace E2.Tastee.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetBaseUrl(this ControllerBase controller)
        {
            return $"{controller.Request.Scheme}://{controller.Request.Host}";
        }
    }
}