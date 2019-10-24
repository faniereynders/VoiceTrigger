using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace VoiceTrigger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var config = builder.Build();
                    var triggersFile = config["TriggersFile"];

                    if (triggersFile != null)
                    {
                        if (File.Exists(triggersFile))
                        {
                            builder.AddJsonFile(triggersFile);
                        }
                        else
                        {
                            throw new FileNotFoundException($"Trigger file {triggersFile} not found.");
                        }
                    }
                    //config = builder.Build();
                    //if (config["MsCog:SubscriptionId"] == null)
                    //{
                    //    throw new ArgumentNullException("MsCog:SubscriptionId");
                    //}
                    //if (config["MsCog:Region"] == null)
                    //{
                    //    throw new ArgumentNullException("MsCog:Region");
                    //}
                })
                .UseStartup<Startup>();
    }
}
