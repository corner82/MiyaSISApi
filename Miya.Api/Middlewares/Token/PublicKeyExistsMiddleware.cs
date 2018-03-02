using Miya.Core.Entities.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Miya.Core.Extensions;

namespace Miya.Middlewares.Token
{
    public class PublicKeyExistsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _distributedCache;
        public PublicKeyExistsMiddleware(RequestDelegate next,
                                         IDistributedCache distributedCache)
        {
            _next = next;
            _distributedCache = distributedCache;
        }

        public async Task Invoke(HttpContext context)


        {
            context.GetHmacToken();
            if (context.Request.Headers.ContainsKey("X-PublicKey"))
            {
                var publicKey = context.Request.Headers.Where(x => x.Key == "X-PublicKey").FirstOrDefault();
                //if(await _distributedCache.)
                if(!string.IsNullOrEmpty(publicKey.Value))
                {
                    SessionUserModel user = new SessionUserModel();
                    try
                    {
                        var cacheUser = await _distributedCache.GetStringAsync(publicKey.Value);
                        try
                        {
                            user = JsonConvert.DeserializeObject<SessionUserModel>(cacheUser);
                        } catch(Exception ex)
                        {
                            //throw
                        }
                        

                        if (user == null)
                        {
                            context.Response.Clear();
                            context.Response.StatusCode = 503;
                            context.Response.ContentType = "application/json";
                            using (var writer = new StreamWriter(context.Response.Body))
                                await writer.WriteLineAsync("{Message : 'User Not Found Due To Public Key'}");
                        }
                        else
                        {
                            context.Items["PublicKey"] = publicKey.Value;
                            var publicKe = context.Items["PublicKey"];
                            await _next(context);
                        }
                    } catch(Exception ex){
                        context.Response.Clear();
                        await context.Response.WriteAsync("redis connection exception");
                    }
                } else
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    using (var writer = new StreamWriter(context.Response.Body))
                        await writer.WriteLineAsync("{Message : 'Public Key Not Found'}");
                }
            } else
            {
                context.Response.Clear();
                context.Response.StatusCode = 503;
                context.Response.ContentType = "application/json";
                using (var writer = new StreamWriter(context.Response.Body))
                    await writer.WriteLineAsync("{Message : 'User Not Found Due To Public Key'}");
            }                
            
        }
    }
}
