using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SSF.Shared;
using SSF.Shared.DTOs;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SSF.MvcFront.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        //Inyectamos IConfiguration en el constructor
        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

            //Leemos la URL del appsettings.json
            _apiUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")
                      ?? "https://localhost:44364/api"; // Una alternativa (fallback) por si te olvidás de ponerla en el JSON
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarLogin([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensaje = "Datos inválidos." });

            try
            {
                // 1. Serializar los datos para mandarlos a la API
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                // 2. Hacer la petición POST a tu API
                var response = await _httpClient.PostAsync($"{_apiUrl}/auth/login", jsonContent);

                // 3. Leer la respuesta de la API (asumiendo que devuelve el Token y los datos del usuario)
                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<GenericResponse<LoginResponseDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { mensaje = apiResponse?.Message ?? "Credenciales incorrectas o error en el servidor." });
                }


                if (apiResponse == null || apiResponse.Data == null || string.IsNullOrEmpty(apiResponse.Data.Token))
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo mapear la respuesta del inicio de sesión. Verificá la estructura.",
                        respuestaCruda = responseString
                    });
                }

                var apiResult = apiResponse.Data;

                // 4. Crear la sesión con Cookies en ASP.NET Core
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, apiResult.Username),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, apiResult.Rol), // "Administrador" mapeará perfecto ahora
                    new Claim("TokenJWT", apiResult.Token)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role
                );

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal
                );

                return Ok(new { redireccion = "/Home/Index" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarRegistro([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensaje = "Datos inválidos." });

            try
            {
                // 1. Obtener el Token del Administrador logueado actualmente desde sus Claims
                var adminToken = User.FindFirst("TokenJWT")?.Value;

                if (string.IsNullOrEmpty(adminToken))
                {
                    return Unauthorized(new { mensaje = "No tenés autorización. Iniciá sesión como Administrador." });
                }

                // 2. Preparar la petición a la API
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/auth/register");
                request.Content = jsonContent;

                // 3. Pasar el Token del Admin a la API en la cabecera
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

                // 4. Enviar
                var response = await _httpClient.SendAsync(request);

                // 5. Leer la respuesta de la API (tanto si fue exitosa como si falló)
                var responseString = await response.Content.ReadAsStringAsync();

                GenericResponse<object>? apiResponse = null;
                try
                {
                    apiResponse = JsonSerializer.Deserialize<GenericResponse<object>>(
                        responseString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch
                {
                    // Si la API no devolvió un JSON válido, apiResponse quedará como null
                }

                // Manejo de Errores de la API
                if (!response.IsSuccessStatusCode)
                {
                    if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Message))
                    {
                        return BadRequest(new { mensaje = apiResponse.Message });
                    }

                    return BadRequest(new { mensaje = "No se pudo registrar el usuario. Error en la comunicación con el servidor." });
                }

                // 6. Respuesta Exitosa
                var mensajeExito = apiResponse?.Message ?? "Usuario registrado exitosamente.";
                return Ok(new { mensaje = mensajeExito });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }

}