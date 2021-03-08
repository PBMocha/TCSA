using GenetecChallenge.N1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WantedListUpdate.Repository;
using WantedListUpdate.Services;

namespace WantedListUpdate
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
            service.AddSingleton<HttpClient>();
            service.AddTransient<LicenseApiService>();
            service.AddTransient<WantedBusService>();


            return service.BuildServiceProvider();

        }
    }
}
