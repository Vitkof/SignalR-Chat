using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Server.Auth;
using Server.Hubs;
using System;
using System.Threading.Tasks;

namespace Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration config)
        {
            _configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<FakeUsers>(_configuration.GetSection("FakeUsers"));

            services.AddControllers();
            services.AddMemoryCache();
            services.AddSignalR(o => o.EnableDetailedErrors = true);
            services.AddCors();
            services.AddSingleton<ChatManager>();

            #region Token Bearer
            /// services.AddJwtAuthentication("./Auth", "rsaKey.json", "noobs", "IServerI");
            #endregion
            services.AddJwtAuthentication();
            services.AddJwtAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCors(policy =>
            {
                policy.SetIsOriginAllowed(origin => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/messages");
            });
        }
    }
}
