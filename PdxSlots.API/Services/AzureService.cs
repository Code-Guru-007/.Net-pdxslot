using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PdxSlots.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PdxSlots.API.Services
{
    public class AzureService : IAzureService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> CreateBlob(byte[] byteData, string fileName, string containerName)
        {
            var container = await GetContainer(containerName);
            BlobClient blobClient = container.GetBlobClient(fileName);
            using (var stream = new MemoryStream(byteData, writable: false))
            {
                await blobClient.UploadAsync(stream, true);
                string fileUrl = blobClient.Uri.ToString();

                return fileUrl;
            }
        }

        public async Task<string> CreateBlobFromFormFile(IFormFile formFile, string containerName)
        {
            var container = await GetContainer(containerName);

            string cleansedFileName = Regex.Replace(formFile.FileName, @"\s", "");
            string uniqueName = $"{Guid.NewGuid()}-{cleansedFileName}";
            BlobClient blobClient = container.GetBlobClient(uniqueName);
            using (Stream stream = formFile.OpenReadStream())
            {
                Azure.Response<BlobContentInfo> uploadResult = await blobClient.UploadAsync(stream, true);

                return blobClient.Uri.ToString();
            }
        }

        public async Task<BlobContainerClient> GetContainer(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return containerClient;
        }
    }
}
