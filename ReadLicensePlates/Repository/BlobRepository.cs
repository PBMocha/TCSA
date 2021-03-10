using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GenetecChallenge.N1.Repository
{
    public class BlobRepository
    {
        private readonly BlobContainerClient _container;
        private readonly BlobClient _blobClient;
        public BlobRepository(IConfiguration config)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(config["TableConnectionString"]);
            
            string containerName = "imagecontainer";

            _container = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> Upload(string path)
        {
            
            var blobClient = _container.GetBlobClient($"{Guid.NewGuid()} {path}");
            using FileStream uploadStream = File.OpenRead(path);
            await blobClient.UploadAsync(uploadStream);

            return blobClient.Uri.ToString();
        }

        

    }
}
