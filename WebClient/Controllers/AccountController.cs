using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebClient.DTOs;

namespace WebClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginDto, string returnUrl = "/")
        {
            if (!ModelState.IsValid) return View(loginDto);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/auth/login", loginDto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(loginDto);
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>(_options);

            // Создаем "удостоверение" пользователя (Claims)
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, loginDto.Email),
                new("access_token", authResponse.Token),
                new(ClaimTypes.Role, authResponse.Role) 
            };
// Добавляем имя, только если оно не пустое
            if (!string.IsNullOrWhiteSpace(authResponse.FullName))
            {
                claims.Add(new Claim(ClaimTypes.Name, authResponse.FullName));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Запомнить пользователя
                RedirectUri = this.Request.Host.Value
            };

            // "Впускаем" пользователя в систему, создавая cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return LocalRedirect(returnUrl);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // "Выпускаем" пользователя, удаляя cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}