using GenetecChallenge.N1;
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

            var _wantedService = serviceProvider.GetService<WantedBusService>();
            var _licenseService = serviceProvider.GetService<LicenseApiService>();
            var _azureRepository = serviceProvider.GetService<LicensePlateRepository>();

            var wanted = await _licenseService.GetWantedList();

            //Store wanted into table
            foreach (var w in wanted)
            {
                Console.WriteLine(w);
                await _azureRepository.StoreLicensePlate(w);
            }

            await _wantedService.UpdateWantedList();

        }

        public static ServiceProvider BuildContainer(ServiceCollection service, IConfiguration config)
        {
            service.AddSingleton(config);
            service.AddSingleton<SecretsConfig>();
            service.AddSingleton<HttpClient>();
            service.AddTransient<LicenseApiService>();
            service.AddTransient<WantedBusService>();
            service.AddSingleton<LicensePlateRepository>();

            return service.BuildServiceProvider();

        }
    }
}
