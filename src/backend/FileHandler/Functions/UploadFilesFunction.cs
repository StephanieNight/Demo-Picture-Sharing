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
    public class UploadFilesFunction
    {
        private StorageService _storageService;
        private string _container;
        public UploadFilesFunction(StorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _container = Environment.GetEnvironmentVariable("StorageContainer");
        }
        [FunctionName("UploadFiles")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            IFormFileCollection files = req.Form.Files;

            await _storageService.UploadFiles(files, _container);

            return new OkObjectResult("File uploaded successfully.");
        }
    }
}
