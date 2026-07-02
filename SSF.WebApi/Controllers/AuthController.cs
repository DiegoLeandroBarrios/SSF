using Microsoft.AspNetCore.Mvc;
using SSF.Domain.Interfaces;
using SSF.Shared.DTOs;
using SSF.Shared;
using Microsoft.AspNetCore.Authorization;

namespace SSF.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyectamos el servicio de autenticación que programamos en Identity
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {

            //Llamamos al servicio para procesar el ingreso
            var respuesta = await _authService.LoginAsync(request);

            //Si devuelve null, significa que el usuario no existe o la clave está mal
            if (!respuesta.Success)
            {
                return BadRequest(respuesta); //Ahora viaja el JSON completo con su estado y mensaje
            }

            //Si todo está OK, devolvemos el Token y los datos del usuario con un estado 200 OK
            return Ok(respuesta);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var resultado = await _authService.RegisterAsync(request);

            if (!resultado.Success)
            {
                return BadRequest(resultado); 
            }

            return Ok(resultado);
        }
    }
}