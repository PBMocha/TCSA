using Azure.Messaging.ServiceBus;
using GenetecChallenge.N1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WantedListUpdate.Repository;

namespace GenetecChallenge.N1.Services
{
    public class LicensePlateBusService
    {
        private readonly SecretsConfig _config;
        private readonly string _connectionString;
        private readonly string _readTopicName;
        private readonly string _subscriptionKey;
        private readonly LicenseApiService _licenseService;
        private readonly LicensePlateRepository _repo;
        public LicensePlateBusService(SecretsConfig config, LicenseApiService licenseService, LicensePlateRepository repo)
        {
            _config = config;

            _connectionString = config.ReadConnectionString;
            _readTopicName = config.ReadTopic;
            _subscriptionKey = config.SubscriptionKey;
            _licenseService = licenseService;
            _repo = repo;

        }
        public async Task SendMessage(LicensePlatePayload licensePlate)
        {
            var json = JsonSerializer.Serialize(licensePlate);
            

            await using (ServiceBusClient client = new ServiceBusClient(_connectionString))
            {
                // create a sender for the topic
                ServiceBusSender sender = client.CreateSender(_readTopicName);
                await sender.SendMessageAsync(new ServiceBusMessage(json));
                Console.WriteLine($"Sent a single message to the topic: {_readTopicName}");
            }
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

            var wantedList = _repo.RetrieveWantedList();

            var lp = JsonSerializer.Deserialize<LicensePlatePayload>(body);

            if (wantedList.Contains(lp.LicensePlate))
            {
                var code = await _licenseService.SendPlate(lp);
                Console.WriteLine($"Sending:\n{lp.LicensePlateCaptureTime}\n{lp.LicensePlate}\n{lp.Longitude}\n{lp.Latitude}\nResponse: {code}");
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
