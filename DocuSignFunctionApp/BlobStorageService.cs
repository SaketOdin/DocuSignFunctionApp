using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobFileService
{
    private readonly string _blobConnectionString;
    private readonly string _containerName;

    public BlobFileService(string blobConnectionString, string containerName)
    {
        _blobConnectionString = blobConnectionString;
        _containerName = containerName;
    }

    public async Task<string> DownloadBlobToTempFileAsync(string blobName)
    {
        // Create a BlobServiceClient
        var blobServiceClient = new BlobServiceClient(_blobConnectionString);

        // Get the container client
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Get the blob client
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob '{blobName}' does not exist in container '{_containerName}'.");
        }

        // Download to a temporary file
        string tempFilePath = Path.GetTempFileName();
        await blobClient.DownloadToAsync(tempFilePath);

        return tempFilePath;
    }
    public  string GetBlobAsBase64Async(string blobName)
    {
        // Create blob client
        var blobServiceClient = new BlobServiceClient(_blobConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!blobClient.Exists())
        {
            throw new FileNotFoundException($"Blob '{blobName}' does not exist in container '{_containerName}'.");
        }

        // Download content to memory stream
        using var memoryStream = new MemoryStream();
        blobClient.DownloadTo(memoryStream);
        var bytes = memoryStream.ToArray();

        // Convert to base64
        return Convert.ToBase64String(bytes);
    }
}

