using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenetecChallenge.N1
{
    public class SecretsConfig
    {
        public string ReadConnectionString { get; }
        public string WantedConnectionString { get; }
        public string ReadTopic { get; }
        public string WantedTopic { get; }
        public string SubscriptionKey { get; }
        public string LicenseRoot { get; }
        public string ApiKey { get; }

        public SecretsConfig(IConfiguration config)
        {
            ReadConnectionString = config["ReadConnectionString"];
            WantedConnectionString = config["WantedConnectionString"];
            ReadTopic = config["ReadTopic"];
            WantedTopic = config["WantedTopic"];
            SubscriptionKey = config["SubscriptionKey"];
            LicenseRoot = config["ApiUrl"];
            ApiKey = config["ApiKey"];
        }

    }
}
