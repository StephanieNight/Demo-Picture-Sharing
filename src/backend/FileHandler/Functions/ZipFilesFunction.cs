using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FileHandler.Functions
{
    public class ZipFilesFunction
    {
        private readonly StorageService _storageService;
        private readonly string _container;
        public ZipFilesFunction(StorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _container = Environment.GetEnvironmentVariable("StorageContainer");
        }
        [FunctionName("GenerateZip")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Generating Zip");
            var fileStream = await _storageService.ZipAllFiles(_container);
            return new FileStreamResult(fileStream, "application/zip")
            {
                FileDownloadName = "pictures.zip"
            };
        }
    }
}
