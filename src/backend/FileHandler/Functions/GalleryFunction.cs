using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Threading.Tasks;

namespace FileUpload.Functions
{
    public class GalleryFunction
    {
        private StorageService _storageService;
        private string _container;

        public GalleryFunction(StorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _container = Environment.GetEnvironmentVariable("StorageContainer");
        }
        [FunctionName("Gallery")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Getting the gallery");
            var result = await _storageService.GetUrisForAllBlobs(_container); 
        
            return new OkObjectResult(result);

        }
    }
}
