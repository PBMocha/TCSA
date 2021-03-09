using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Tables;
using Microsoft.Azure.Cosmos.Table;
using GenetecChallenge.N1.Models;
using System.Threading.Tasks;

namespace WantedListUpdate.Repository
{
    public class LicensePlateRepository
    {
        private readonly string _tableConnectionString;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        public LicensePlateRepository(IConfiguration config)
        {
            _tableConnectionString = config["TableConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_tableConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference("WantedList");
            _table.CreateIfNotExists();
        }
        public async Task StoreLicensePlate(string lp) 
        {
            try
            {
                var lpdto = new WantedLicensePlate
                {
                    LicensePlate = lp
                };
                lpdto.AssignPartitionKey();
                lpdto.AssignRowKey();

                TableOperation op = TableOperation.Insert(lpdto);
                await _table.ExecuteAsync(op);
            } catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<string> RetrieveWantedList()
        {
            var op = new TableQuery<WantedLicensePlate>();
            var list = new List<string>();
            foreach(var plate in _table.ExecuteQuery(op))
            {
                list.Add(plate.LicensePlate);
            }
            return list;
        }

        
    }
}
