using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miya.Core.Utills;
using Miya.Core.Entities.Session;
using Miya.Api.Filters;
using Miya.Api.Middlewares.Auth.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wangkanai.Detection;
using Miya.Core.Utills.Url;
using Miya.Api.Middlewares.Token;
using Miya.Api.Middlewares.User;

namespace Miya.Core
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            /*var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("miyasettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"miyasettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();*/
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<HmacTokenControllerAttribute>();
            services.AddScoped<HmacFilterAttribute>();
            services.AddScoped<SessionUserModel, SessionUserModel>();

            // redis ayarları
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = Configuration.GetConnectionString("RedisInstanceName");
                options.Configuration = Configuration.GetConnectionString("RedisServer");
            }
            );
            // session ayarları
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie = new CookieBuilder
                {
                    //Expiration = TimeSpan.FromMinutes(2),
                    HttpOnly = false,
                    Domain = "http://localhost/9082/",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest,
                    SameSite = SameSiteMode.Lax,
                    Name = ".Miya.Security.Cookie",
                    //Path = "/",
                };
            });

            // Add detection services container and device resolver service.(Wangkanai.detection)
            services.AddDetectionCore()
                    .AddDevice();

            services.AddMvc();


            services.AddScoped<RemoteAddressFinder, RemoteAddressFinder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //Use session
            app.UseSession();

            

            //session user set middleware
            app.UseMiddleware<PublicKeyExistsMiddleware>();
            app.UseMiddleware<SetUserToken>();
            app.UseMiddleware<SetUserMiddleware>();
            
            app.UseMvc();
        }
    }
}
