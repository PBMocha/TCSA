using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenetecChallenge.N1.Models
{
    class WantedLicensePlate : TableEntity
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }

        public void AssignRowKey()
        {
            RowKey = Id.ToString();
        }

        public void AssignPartitionKey()
        {
            PartitionKey = LicensePlate;
        }
    }
}
