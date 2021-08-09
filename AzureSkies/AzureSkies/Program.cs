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


namespace AzureSkies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

            SmsClient smsClient = new SmsClient(connectionString);

            SmsSendResult sendResult = smsClient.Send(
                from: "+18443976066",
                to: "+12158507772",
                message: "URL incoming"
                );

            Console.WriteLine($"Sms id: {sendResult.MessageId}");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
