using bd2_first_api_202603.DTOs;
using bd2_first_api_202603.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace bd2_first_api_202603.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var id = await _auth.RegisterAsync(dto);
                return Created(string.Empty, new { IdUsuario = id, Mensaje = "Usuario registrado." });
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { Mensaje = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _auth.LoginAsync(dto);
            if (result is null)
                return Unauthorized(new { Mensaje = "Credenciales inválidas." });

            return Ok(result);
        }

        [HttpPost("asignar-rol")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AsignarRol(AsignarRolDto dto)
        {
            try
            {
                await _auth.AsignarRolAsync(dto);
                return Ok(new { Mensaje = "Rol asignado." });
            }
            catch (SqlException ex) when (ex.Number is 50002 or 50003)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                Id = User.FindFirst("sub")?.Value,
                Username = User.Identity?.Name,
                Roles = User.FindAll("role").Select(c => c.Value)
            });
        }
    }
}
