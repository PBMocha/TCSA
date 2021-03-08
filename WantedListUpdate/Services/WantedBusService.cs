using Azure.Messaging.ServiceBus;
using GenetecChallenge.N1;
using GenetecChallenge.N1.Models;
using GenetecChallenge.N1.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WantedListUpdate.Repository;

namespace WantedListUpdate.Services
{
    public class WantedBusService
    {
        private readonly SecretsConfig _config;
        private readonly string _connectionString;
        private readonly string _readTopicName;
        private readonly string _subscriptionKey;
        private readonly LicenseApiService _licenseService;
        private readonly LicensePlateRepository _azureRepository;
        public WantedBusService(SecretsConfig config, LicensePlateRepository azureRepository,LicenseApiService licenseService)
        {
            _config = config;

            _connectionString = config.WantedConnectionString;
            _readTopicName = config.WantedTopic;
            _subscriptionKey = config.SubscriptionKey;
            _licenseService = licenseService;
            _azureRepository = azureRepository;

        }
        public async Task ReceiveLicensePlates()
        {
            await using (ServiceBusClient client = new ServiceBusClient(_connectionString))
            {
                // create a processor that we can use to process the messages
                ServiceBusProcessor processor = client.CreateProcessor(_readTopicName, _subscriptionKey, new ServiceBusProcessorOptions());

                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
        }

        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            var lp = JsonSerializer.Deserialize<LicensePlatePayload>(body);

            var code = await _licenseService.SendPlate(lp);
            Console.WriteLine($"Sending:\n{lp.LicensePlateCaptureTime}\n{lp.LicensePlate}\n{lp.Longitude}\n{lp.Latitude}\nResponse: {code}");

            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}
