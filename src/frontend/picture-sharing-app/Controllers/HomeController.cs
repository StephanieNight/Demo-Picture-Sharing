using Microsoft.AspNetCore.Mvc;
using picture_sharing.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace picture_sharing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = config;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {

            return View();
        }
        public async Task<IActionResult> Fullscreen(Uri url)
        {
            return View(url);
        }
        public async Task<IActionResult> Gallery()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files.Count == 0) { return StatusCode(400, "Files not found"); }
            try
            {
                // Your file upload logic here

                // Assuming you want to send the file to this external endpoint
                var url = _configuration.GetSection("BackendURL").Value;

                var chunkSize = 5;

                var totalFiles = files.Count;
                var startIndex = 0;

                while(startIndex < totalFiles)
                {
                    // Create a new MultipartFormDataContent
                    var multipartContent = new MultipartFormDataContent();

                    // Add each file as content to the multipart content
                    for (var i = startIndex; i < Math.Min(startIndex + chunkSize, totalFiles); i++)
                    {
                        var file = files[i];
                        var fileContent = new StreamContent(file.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        multipartContent.Add(fileContent, "files", file.FileName);
                    }

                    // Send the request to the external endpoint
                    var response = await _httpClient.PostAsync($"{url}/UploadFiles", multipartContent);

                    // Check if the request was successful
                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, $"Error uploading files to external endpoint");
                    }
                    startIndex += chunkSize;
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading files: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        public async Task<IActionResult> DownloadFile(Uri url)
        {
            var urlchunks = url.ToString().Split("?");
            var filepath = urlchunks[0];
            var filepathchunks = filepath.Split("/");
            var filename = filepathchunks[filepathchunks.Length - 1];

            return File(await DownloadFileAsync(url), "application/octet-stream", filename);
        }

        private async Task<byte[]> DownloadFileAsync(Uri url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        [HttpGet]
        public async Task<List<Uri>> GetGalleryUris() 
        {
            var url = _configuration.GetSection("BackendURL").Value;
            var response = await _httpClient.GetAsync($"{url}/Gallery");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Uri>>();
            }
            return new List<Uri>();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
