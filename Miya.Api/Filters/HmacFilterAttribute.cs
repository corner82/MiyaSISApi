using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Miya.Api.Filters
{
    public class HmacFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //var user = _httpContext.GetMiyaUser();
            if(context!=null)
            {

                if(!context.HttpContext.Request.Headers.ContainsKey("X-Hmac"))
                {
                    context.Result = new StatusCodeResult(405);
                }
            }
        }
    }
}
