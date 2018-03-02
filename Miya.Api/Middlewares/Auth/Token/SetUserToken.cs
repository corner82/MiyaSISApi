using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miya.Api.Middlewares.Auth.Token
{
    public class SetUserToken
    {
        private readonly RequestDelegate _next;
        public SetUserToken(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var headerList = context.Request.Headers.ToList();
            var xHmac = headerList.Where(x => x.Key == "X-Hmac").FirstOrDefault();
            if(!string.IsNullOrEmpty(xHmac.Value) && !string.IsNullOrEmpty(xHmac.Key))
            {
                context.Items["HmacToken"] = xHmac.Value;
            }
            await _next(context);
        }
    }
}
