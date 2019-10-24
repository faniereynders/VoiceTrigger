using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VoiceTrigger
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            if (Configuration.GetChildren().Any(i => i.Key == "MsCog"))
            {
                Console.WriteLine("Recognizer: MS Cognitive Services");
                services.AddSingleton<ISpeechRecognitionProvider, CognitiveServicesRecognitionProvider>();
            }
            else
            {
                Console.WriteLine("Recognizer: System.Speech");
                services.AddSingleton<ISpeechRecognitionProvider, SystemSpeechRecognitionProvider>();
            }


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
