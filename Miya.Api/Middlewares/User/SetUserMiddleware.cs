using Miya.Core.Entities.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Miya.Api.Middlewares.User
{
    public class SetUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _distributedCache;
        private SessionUserModel _sessionUserModel;
        public SetUserMiddleware(RequestDelegate next, 
                                 IDistributedCache distributedCache
                                 )
        {
            _next = next;
            _distributedCache = distributedCache;
            
        }

        public async Task Invoke(HttpContext context, SessionUserModel sessionUserModel)
        {
            _sessionUserModel = sessionUserModel;
            var headerList = context.Request.Headers.ToList();
            var xPublic = headerList.Where(x => x.Key == "X-PublicKey").FirstOrDefault();
            if (!string.IsNullOrEmpty(xPublic.Value))
            {
                var publicKey = context.Items["PublicKey"].ToString();
                if(!string.IsNullOrEmpty(publicKey))
                {
                    
                    var userTestObj = _distributedCache.GetString(publicKey);
                    _sessionUserModel = JsonConvert.DeserializeObject<SessionUserModel>(userTestObj);
                    if (_sessionUserModel != null)
                    {
                        context.Items["MiyaUser"] = _sessionUserModel;
                        context.Items["MiyaUserName"] = _sessionUserModel.Email;
                        context.Items["MiyaUserPassword"] = _sessionUserModel.Password;
                        context.Items["PrivateKey"] = _sessionUserModel.SecurityStamp;
                        context.Items["UserAgent"] = _sessionUserModel.UserAgent;
                    }
                    var dene = _sessionUserModel.Email;
                    await _next(context);
                } else
                {
                    context.Response.StatusCode = 503;
                    context.Response.ContentType = "application/json";
                    using (StreamWriter writer = new StreamWriter(context.Response.Body))
                    {
                        await writer.WriteLineAsync("{Message : 'User Not Found Due To Public Key'}");
                    };
                }
            } else
            {
                //await _next(context);
            }
        }
    }
}
