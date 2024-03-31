using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using picture_sharing.Models;
using System.Diagnostics;

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
        public async Task<IActionResult> Gallery()
        {
            var url = _configuration.GetSection("BackendURL").Value;
            var response = await _httpClient.GetAsync($"{url}/Gallery");
            if (response.IsSuccessStatusCode)
            {
                var images = await response.Content.ReadFromJsonAsync<List<Uri>>();
                return View(images);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
