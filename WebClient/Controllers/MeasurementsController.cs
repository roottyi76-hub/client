using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using WebClient.DTOs.Laser;
using WebClient.DTOs.Measurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebClient.Controllers
{
    [Authorize]
    public class MeasurementsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public MeasurementsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Measurements
        public async Task<IActionResult> Index(string sortOrder)
        {
            // ViewData используется для передачи параметров сортировки в представление
            ViewData["IntensitySortParm"] = string.IsNullOrEmpty(sortOrder) ? "intensity_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["LaserIdSortParm"] = sortOrder == "LaserId" ? "laserid_desc" : "LaserId";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "id_desc" : "Id";

            var client = _httpClientFactory.CreateClient("ApiClient");
            var measurements = await client.GetFromJsonAsync<List<MeasurementDto>>("api/measurements", _options) ?? new List<MeasurementDto>();

            // Логика сортировки на основе параметра sortOrder
            switch (sortOrder)
            {
                case "intensity_desc":
                    measurements = measurements.OrderByDescending(m => m.Intensity).ToList();
                    break;
                case "Date":
                    measurements = measurements.OrderBy(m => m.MeasurementDate).ToList();
                    break;
                case "date_desc":
                    measurements = measurements.OrderByDescending(m => m.MeasurementDate).ToList();
                    break;
                case "LaserId":
                    measurements = measurements.OrderBy(m => m.LaserId).ToList();
                    break;
                case "laserid_desc":
                    measurements = measurements.OrderByDescending(m => m.LaserId).ToList();
                    break;
                case "Id":
                    measurements = measurements.OrderBy(m => m.MeasurementId).ToList();
                    break;
                case "id_desc":
                    measurements = measurements.OrderByDescending(m => m.MeasurementId).ToList();
                    break;
                default: // Сортировка по умолчанию
                    measurements = measurements.OrderBy(m => m.Intensity).ToList();
                    break;
            }

            return View(measurements);
        }

        // Вспомогательный метод для загрузки лазеров
        private async Task LoadLasersToViewBag()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var lasers = await client.GetFromJsonAsync<List<LaserDto>>("api/lasers", _options);
            ViewBag.Lasers = new SelectList(lasers, "LaserId", "LaserId");
        }

        // GET: /Measurements/Create
        public async Task<IActionResult> Create()
        {
            await LoadLasersToViewBag();
            return View();
        }

        // POST: /Measurements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeasurementCreateUpdateDto measurementDto)
        {
            // Устанавливаем время в UTC перед отправкой
            measurementDto.MeasurementDate = measurementDto.MeasurementDate.ToUniversalTime();

            if (!ModelState.IsValid)
            {
                await LoadLasersToViewBag();
                return View(measurementDto);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(measurementDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/measurements", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Ошибка сервера при создании измерения.");
            await LoadLasersToViewBag();
            return View(measurementDto);
        }
        
        // GET: /Measurements/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            // API должен иметь метод GetMeasurement(id)
            var response = await client.GetAsync($"api/measurements/{id}"); 
            if (!response.IsSuccessStatusCode) return NotFound();
            
            var measurement = await response.Content.ReadFromJsonAsync<MeasurementDto>(_options);
            
            var updateDto = new MeasurementCreateUpdateDto
            {
                Intensity = measurement.Intensity,
                MeasurementDate = measurement.MeasurementDate,
                LaserId = measurement.LaserId
            };

            await LoadLasersToViewBag();
            ViewBag.MeasurementId = measurement.MeasurementId;
            return View(updateDto);
        }

        // POST: /Measurements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeasurementCreateUpdateDto measurementDto)
        {
            measurementDto.MeasurementDate = measurementDto.MeasurementDate.ToUniversalTime();

            if (!ModelState.IsValid)
            {
                await LoadLasersToViewBag();
                return View(measurementDto);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(measurementDto), Encoding.UTF8, "application/json");
            await client.PutAsync($"api/measurements/{id}", content);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Measurements/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/measurements/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var measurement = await response.Content.ReadFromJsonAsync<MeasurementDto>(_options);
            return View(measurement);
        }

        // POST: /Measurements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            await client.DeleteAsync($"api/measurements/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}

