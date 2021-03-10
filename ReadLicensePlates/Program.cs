using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using GenetecChallenge.N1.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GenetecChallenge.N1.Services;
using System.Net.Http;
using WantedListUpdate.Repository;
using GenetecChallenge.N1.Repository;

namespace GenetecChallenge.N1
{
    class Program
    {

        private static IConfiguration _config { get; } = new ConfigurationBuilder().AddJsonFile("config.json").Build();

        public static async Task Main(string[] args)
        {

            Console.WriteLine("TCSA!");

            var serviceProvider = BuildContainer(new ServiceCollection(), _config);

            var _lpService = serviceProvider.GetService<LicensePlateBusService>();
            
            await _lpService.ReceiveLicensePlates();
            


        }

        public static ServiceProvider BuildContainer(ServiceCollection service, IConfiguration config)
        {
            service.AddSingleton(config);
            service.AddSingleton<SecretsConfig>();
            service.AddSingleton<HttpClient>();
            service.AddTransient<LicenseApiService>();
            service.AddTransient<LicensePlateBusService>();
            service.AddSingleton<LicensePlateRepository>();
            service.AddScoped<BlobRepository>();
            service.AddScoped<FileRepository>();
            return service.BuildServiceProvider();

        }

    }
}
