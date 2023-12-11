
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.CodeAnalysis.CSharp;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobClient;
        
        public ContainerService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();

        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerNamesAndBlobNames = new();
            containerNamesAndBlobNames.Add("Account Name : " + _blobClient.AccountName);
            containerNamesAndBlobNames.Add("-------------------------------------------------------------");
            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerNamesAndBlobNames.Add("--" + blobContainerItem.Name);
                BlobContainerClient _blobContainer =
                    _blobClient.GetBlobContainerClient(blobContainerItem.Name);

            }
        }

        public async Task<List<string>> GetAllContainers()
        {
            List<string> containerName = new();

            await foreach(BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerName.Add(blobContainerItem.Name);
            }

            return containerName;
        }
    }
}
