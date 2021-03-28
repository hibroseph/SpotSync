using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotSync.Classes.Authorization;
using SpotSync.Classes.Hubs;
using SpotSync.Classes.Middleware;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Events;

namespace SpotSync
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAndStartDatabaseMigration(Configuration);

            services.AddControllersWithViews();
            services.AddSpotSyncServices(Configuration);
            services.AddSpotSyncAuthentication();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DiagnosticsPolicy", policyBuilder =>
                {
                    policyBuilder.AddRequirements(new DiagnosticsKeyRequirement(Configuration["DiagnosticsApiKey"]));
                });
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogService logService)
        {
            DomainEvents.Configure(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                new ConfigurationBuilder().AddUserSecrets("faf3a1a4-4c57-48a4-91df-e775496e02d5").Build();
            }
            else
            {
                app.UseMiddleware<ExceptionHandler>();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<PartyHub>("/partyhub");
            });
        }
    }
}
