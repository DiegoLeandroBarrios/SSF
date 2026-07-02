using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SSF.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        // 🔒 Este atributo es el candado. Solo deja pasar a usuarios con Token JWT válido.
        [Authorize]
        [HttpGet("lista")]
        public IActionResult GetListaProductos()
        {
            // Simulamos una lista de productos para la prueba
            var productos = new[]
            {
                new { Id = 1, Nombre = "Computadora Portátil HP", Precio = 850000 },
                new { Id = 2, Nombre = "Memoria RAM DDR4 16GB", Precio = 45000 }
            };

            return Ok(new { Mensaje = "Acceso concedido a los datos protegidos.", Datos = productos });
        }

        // 👑 Este endpoint es aún más estricto: Solo para el rol Administrador
        [Authorize(Roles = "Administrador")]
        [HttpGet("reporte-ventas")]
        public IActionResult GetReportePrivado()
        {
            return Ok("¡Hola DiegoAdmin! Este reporte confidencial de ventas solo lo puede ver un Administrador.");
        }
    }
}