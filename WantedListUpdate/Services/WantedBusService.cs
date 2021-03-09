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
        public WantedBusService(SecretsConfig config, LicensePlateRepository azureRepository, LicenseApiService licenseService)
        {
            _config = config;

            _connectionString = config.WantedConnectionString;
            _readTopicName = config.WantedTopic;
            _subscriptionKey = config.SubscriptionKey;
            _licenseService = licenseService;
            _azureRepository = azureRepository;

        }
        public async Task UpdateWantedList()
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

                Console.WriteLine("Waiting for updates...");
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

            Console.WriteLine($"{body}");

            var wanted = await _licenseService.GetWantedList();
            
            //Store wanted into table
            foreach(var w in wanted)
            {
                await _azureRepository.StoreLicensePlate(w);
            }

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
