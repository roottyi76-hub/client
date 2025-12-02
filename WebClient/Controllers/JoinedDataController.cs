using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using WebClient.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;

namespace WebClient.Controllers
{
    [Authorize]
    public class JoinedDataController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public JoinedDataController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["IntensitySortParm"] = string.IsNullOrEmpty(sortOrder) ? "intensity_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["LaserIdSortParm"] = sortOrder == "LaserId" ? "laserid_desc" : "LaserId";
            ViewData["XSortParm"] = sortOrder == "X" ? "x_desc" : "X";
            ViewData["YSortParm"] = sortOrder == "Y" ? "y_desc" : "Y";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "id_desc" : "Id";
            
            var client = _httpClientFactory.CreateClient("ApiClient");
            var data = await client.GetFromJsonAsync<List<JoinedDataDto>>("api/joineddata", _options) ?? new List<JoinedDataDto>();
            
            switch (sortOrder)
            {
                case "intensity_desc":
                    data = data.OrderByDescending(d => d.Intensity).ToList();
                    break;
                case "Date":
                    data = data.OrderBy(d => d.MeasurementDate).ToList();
                    break;
                case "date_desc":
                    data = data.OrderByDescending(d => d.MeasurementDate).ToList();
                    break;
                case "LaserId":
                    data = data.OrderBy(d => d.LaserId).ToList();
                    break;
                case "laserid_desc":
                    data = data.OrderByDescending(d => d.LaserId).ToList();
                    break;
                case "X":
                    data = data.OrderBy(d => d.LaserX).ToList();
                    break;
                case "x_desc":
                    data = data.OrderByDescending(d => d.LaserX).ToList();
                    break;
                case "Y":
                    data = data.OrderBy(d => d.LaserY).ToList();
                    break;
                case "y_desc":
                    data = data.OrderByDescending(d => d.LaserY).ToList();
                    break;
                case "Id":
                    data = data.OrderBy(d => d.MeasurementId).ToList();
                    break;
                case "id_desc":
                    data = data.OrderByDescending(d => d.MeasurementId).ToList();
                    break;
                default:
                    data = data.OrderBy(d => d.Intensity).ToList();
                    break;
            }

            return View(data);
        }
    }
}

