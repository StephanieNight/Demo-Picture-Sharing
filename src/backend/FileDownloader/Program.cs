using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using picture_sharing.Services;

namespace FileDownload
{
    internal class Program
    {
        private static readonly string _keyvault = "kvdevpictureshare";
        static void Main(string[] args)
        {
            DownloadAllBlobsFromContainerAsync("pictures", "D:\\test-download").Wait();
           
        }
        public static async Task DownloadAllBlobsFromContainerAsync(string containerName, string destinationFolder)
        {
            var keyVaultUri = $"https://{_keyvault}.vault.azure.net";
            var keyVaultService =new KeyVaultService(keyVaultUri);
            string connectionString = keyVaultService.GetSecret("storage-constring");
            
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            Console.WriteLine("Connected!..");

            // Ensure the destination folder exists
            Directory.CreateDirectory(destinationFolder);

            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                string destinationPath = Path.Combine(destinationFolder, blobItem.Name);
                
                Console.Write($"Downloading : {blobItem.Name}...");

                // Download the blob to a local file
                using FileStream fileStream = File.Create(destinationPath);
                await blobClient.DownloadToAsync(fileStream);

                Console.WriteLine("Success!..");
            }
        }
    }
}
