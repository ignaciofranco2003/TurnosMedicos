using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Auth.DTOS.Request;
using TurnosMedicos.Auth.Interfaces;

namespace TurnosMedicos.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            await _auth.RegisterAsync(dto);
            return Ok(new { success = true, message = "Usuario registrado correctamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al registrar usuario", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var token = await _auth.LoginAsync(dto);
            return Ok(new { success = true, data = token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "Credenciales inválidas" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al iniciar sesión", error = ex.Message });
        }
    }
}
