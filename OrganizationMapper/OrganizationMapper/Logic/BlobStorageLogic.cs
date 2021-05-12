using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrganizationMapper.Logic
{
    public class BlobStorageLogic
    {
        private readonly string _inputFileName = "Input.csv";
        private readonly string _containerName = "inputfiles";

        public BlobStorageLogic()
        {
        }

        public void UploadBlob(string storageConn, IFormFile file)
        {
            var blobClient = GetBlobClient(storageConn);

            using var stream = file.OpenReadStream();
            blobClient.Upload(stream, true);
        }

        public List<string> GetOrganizationData(string storageConn)
        {
            var blobClient = GetBlobClient(storageConn);

            var returnList = new List<string>();

            using (StreamReader reader = new StreamReader(blobClient.OpenRead()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    returnList.Add(line);
                }
            }

            return returnList;
        }

        private BlobClient GetBlobClient(string storageConn)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConn);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            return containerClient.GetBlobClient(_inputFileName);
        }
    }
}
