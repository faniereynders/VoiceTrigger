using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VoiceTrigger
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddSingleton<IHostedService, TriggerRecognizerBackgroundService>()
                .AddSingleton<ITriggerDispatcher, TriggerDispatcher>();

            services.Configure<Dictionary<string, TriggerOption>>(Configuration.GetSection("Triggers"));
            services.AddSignalR();

            services.ConfigureOptions(typeof(ConfigureOptions));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
                        
            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<VoiceTriggerHub>("/trigger");
            });
            app.UseMvc();
        }
    }
}
