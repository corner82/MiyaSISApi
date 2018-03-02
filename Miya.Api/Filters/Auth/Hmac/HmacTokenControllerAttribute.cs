using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Miya.Core.Extensions;
using Miya.Core.Auth.Hmac;
using Miya.Core.Utills;
using Wangkanai.Detection;
using System;
using Miya.Core.Utills.Url;

namespace Miya.Filters.Auth.Hmac
{
    public class HmacTokenControllerAttribute : ActionFilterAttribute
    {
        private readonly RemoteAddressFinder _remoteAdressFinder;
        private readonly IDeviceResolver _deviceResolver;
        public HmacTokenControllerAttribute(RemoteAddressFinder remoteAdressFinder,
                                            IDeviceResolver deviceResolver)
        {
            _remoteAdressFinder = remoteAdressFinder;
            _deviceResolver = deviceResolver;
        }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var password = context.HttpContext.GetUserPassword();
            var privateKey = context.HttpContext.GetPrivateKey();
            var email = context.HttpContext.GetUserName();
            var userName = context.HttpContext.GetUserName();
            var ip = _remoteAdressFinder.GetRequestIP();
            //var userAgentString = _deviceResolver.UserAgent.ToString();
            var token = context.HttpContext.GetHmacToken();
            var userAgentString = context.HttpContext.GetUserAgent();
            if (!HmacServiceManager.IsTokenValid(userName, privateKey, token, ip, userAgentString))
            {
                context.HttpContext.Response.StatusCode = 403;
                context.HttpContext.Response.ContentType = "Application/Json";
                context.HttpContext.Response.WriteAsync("Invalid Token");
            }
            base.OnActionExecuting(context);
        }
    }
}
