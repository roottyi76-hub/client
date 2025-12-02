using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using WebClient.DTOs.Laser;
using WebClient.DTOs.Measurement;

namespace WebClient.Controllers
{
    [Authorize]
    public class ChartsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration; // 👈 1. Внедрить
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public ChartsController(IHttpClientFactory httpClientFactory, IConfiguration configuration) // 👈 2. Получить
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration; // 👈 3. Сохранить
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult RealTime()
        {
            // 👈 4. Передать URL сервера во View
            // (Он возьмет его из переменной окружения Render)
            ViewBag.ServerBaseUrl = _configuration.GetValue<string>("ApiClient:BaseAddress");
            return View();
        }

        public async Task<IActionResult> LaserPositions()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var lasers = await client.GetFromJsonAsync<List<LaserDto>>("api/lasers", _options);

            // Преобразуем данные для Chart.js (scatter plot)
            var chartData = lasers.Select(l => new { x = l.X, y = l.Y }).ToList();
            ViewBag.ChartDataJson = JsonSerializer.Serialize(chartData);

            return View();
        }

        public async Task<IActionResult> IntensityTrend()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var measurements = await client.GetFromJsonAsync<List<MeasurementDto>>("api/measurements", _options);

            // Преобразуем данные для Chart.js (line chart)
            var sortedData = measurements.OrderBy(m => m.MeasurementDate);
            var chartLabels = sortedData.Select(m => m.MeasurementDate.ToString("dd.MM HH:mm"));
            var chartValues = sortedData.Select(m => m.Intensity);

            ViewBag.ChartLabelsJson = JsonSerializer.Serialize(chartLabels);
            ViewBag.ChartValuesJson = JsonSerializer.Serialize(chartValues);

            return View();
        }
    }
}