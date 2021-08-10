using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Communication;
using Azure.Communication.Sms;
using AzureSkies.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AzureSkies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

            //SmsClient smsClient = new SmsClient(connectionString);

            //SmsSendResult sendResult = smsClient.Send(
            //    from: "+18443976066",
            //    to: "+12158507772",
            //    message: "URL incoming"
            //    );

            //Console.WriteLine($"Sms id: {sendResult.MessageId}");

            var host = CreateHostBuilder(args).Build();

            UpdateDatabase(host.Services);

            host.Run();
        }
        private static void UpdateDatabase(IServiceProvider services)
        {
            using (var serviceScope = services.CreateScope())
            {
                using (var db = serviceScope.ServiceProvider.GetService<AzureSkiesDbContext>())
                {
                    db.Database.Migrate();
                }
            }
        }

        // Part of the Web App, what gets you to the home page.
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
