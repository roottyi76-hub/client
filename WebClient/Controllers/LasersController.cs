using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using WebClient.DTOs.Laser;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebClient.Controllers
{
    [Authorize]
    public class LasersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public LasersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Lasers
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["IdSortParm"] = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["XSortParm"] = sortOrder == "X" ? "x_desc" : "X";
            ViewData["YSortParm"] = sortOrder == "Y" ? "y_desc" : "Y";

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/lasers");
            var lasers = new List<LaserDto>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                lasers = JsonSerializer.Deserialize<List<LaserDto>>(content, _options);
            }

            switch (sortOrder)
            {
                case "id_desc":
                    lasers = lasers.OrderByDescending(l => l.LaserId).ToList();
                    break;
                case "X":
                    lasers = lasers.OrderBy(l => l.X).ToList();
                    break;
                case "x_desc":
                    lasers = lasers.OrderByDescending(l => l.X).ToList();
                    break;
                case "Y":
                    lasers = lasers.OrderBy(l => l.Y).ToList();
                    break;
                case "y_desc":
                    lasers = lasers.OrderByDescending(l => l.Y).ToList();
                    break;
                default:
                    lasers = lasers.OrderBy(l => l.LaserId).ToList();
                    break;
            }
            
            return View(lasers);
        }

        // GET: /Lasers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Lasers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LaserCreateUpdateDto laserDto)
        {
            if (!ModelState.IsValid) return View(laserDto);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(laserDto), Encoding.UTF8, "application/json");
            await client.PostAsync("api/lasers", content);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: /Lasers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/lasers/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();
            
            var content = await response.Content.ReadAsStringAsync();
            var laser = JsonSerializer.Deserialize<LaserDto>(content, _options);
            
            var updateDto = new LaserCreateUpdateDto { X = laser.X, Y = laser.Y };
            ViewBag.LaserId = laser.LaserId;
            return View(updateDto);
        }

        // POST: /Lasers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LaserCreateUpdateDto laserDto)
        {
            if (!ModelState.IsValid) return View(laserDto);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(laserDto), Encoding.UTF8, "application/json");
            await client.PutAsync($"api/lasers/{id}", content);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Lasers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/lasers/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var laser = JsonSerializer.Deserialize<LaserDto>(content, _options);
            return View(laser);
        }

        // POST: /Lasers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            await client.DeleteAsync($"api/lasers/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}

