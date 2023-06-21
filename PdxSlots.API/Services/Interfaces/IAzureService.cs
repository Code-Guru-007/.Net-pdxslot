using Azure.Storage.Blobs;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IAzureService
    {
        Task<string> CreateBlob(byte[] byteData, string fileName, string containerName);
        Task<string> CreateBlobFromFormFile(IFormFile formFile, string containerName);
        Task<BlobContainerClient> GetContainer(string containerName);
    }
}
