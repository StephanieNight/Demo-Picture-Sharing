using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using picture_sharing.Services;

namespace Services
{
    public class StorageService
    {
        private string _connectionString;
        private readonly KeyVaultService _keyVaultService;
        public StorageService(KeyVaultService keyVaultService)
        {
            _keyVaultService = keyVaultService;
            _connectionString = _keyVaultService.GetSecret("storage-constring");
        }
        public async Task UploadFiles(IFormFileCollection files, string containerName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            foreach (var file in files)
            {                
                var stream = file.OpenReadStream();
                var filename = $"IMG_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
      
                //Convert HEIC/HEIF to JPEG
                if (filename.EndsWith(".heic") || filename.EndsWith(".heif"))                
                {
                    // Change the extension
                    filename = Path.GetFileNameWithoutExtension(filename) + ".jpeg";                    
         
                    // Create image that is completely purple and 800x600
                    using (MagickImage image = new MagickImage(stream))
                    {
                        // Sets the output format to jpeg
                        image.Format = MagickFormat.Jpeg;                 

                        // Read the .heic file into a byte array
                        byte[] imageData = image.ToByteArray();

                        // Now you can convert this byte array to a stream if needed
                        stream = new MemoryStream(imageData);                       
                    }                                    
                }
                // Create the blob client with the file name of the file to be written.
                var blobClient = containerClient.GetBlobClient(filename);
                
                // Write the image to the blob client                        
                await blobClient.UploadAsync(stream, true);

                // Remember to clean up your resources.
                stream.Close();
                stream.Dispose();
            }
        }
        
        public async Task<List<Uri>> GetUrisForAllBlobs(string containerName) {

            List<Uri> result = new List<Uri>();

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
;
            await foreach (BlobItem blob in containerClient.GetBlobsAsync())
            {
                // Check if the blob is an image (you can adjust the criteria as needed)
                if (blob.Name.EndsWith(".jpg") || blob.Name.EndsWith(".jpeg") || blob.Name.EndsWith(".png"))
                {
                    result.Add(GetBlobUri(containerClient,blob.Name));
                }
            }
            return result;
        }
        public Uri GetBlobUri(BlobContainerClient containerClient, string blobName)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Generate a SAS token for the blob
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobName,
                Resource = "b", // "b" for blob
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1), // Adjust the expiry time as needed
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

            return sasURI;
        }
    }
}
