using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Services.Interfaces;

namespace TurnosMedicos.Controllers;

[ApiController]
[Route("api/turnos")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Usuario")]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _service;

    public TurnosController(ITurnoService service)
        => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? estado = null)
    {
        try
        {
            var items = await _service.GetAllAsync(estado);
            if (items == null || !items.Any())
                return Ok(new { success = true, message = "No se encontraron turnos", data = items });

            return Ok(new { success = true, data = items });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener turnos", error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var turno = await _service.GetByIdAsync(id);
            return turno is null
                ? NotFound(new { success = false, message = $"No existe turno con ID {id}" })
                : Ok(new { success = true, data = turno });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener el turno", error = ex.Message });
        }
    }

    [HttpGet("estados")]
    public IActionResult GetEstados()
    {
        try
        {
            var estados = _service.GetEstados();
            return Ok(new { success = true, data = estados });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener estados de turno", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TurnoRequestDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, message = "Turno creado correctamente", data = created });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al crear el turno", error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TurnoUpdateRequestDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto);
            return ok
                ? Ok(new { success = true, message = "Turno actualizado correctamente" })
                : NotFound(new { success = false, message = $"No existe turno con ID {id}" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al actualizar el turno", error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id);
            return ok
                ? Ok(new { success = true, message = "Turno eliminado correctamente" })
                : NotFound(new { success = false, message = $"No existe turno con ID {id}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ocurrió un error al eliminar el turno", error = ex.Message });
        }
    }
}
