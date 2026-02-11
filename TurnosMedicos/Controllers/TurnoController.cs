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
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _service.GetAllAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var turno = await _service.GetByIdAsync(id);
            return turno is null ? NotFound() : Ok(turno);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("estados")]
    public IActionResult GetEstados()
    {
        try
        {
            return Ok(_service.GetEstados());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TurnoRequestDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TurnoRequestDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto);
            return ok
                ? Ok(new { message = "Turno actualizado correctamente" })
                : NotFound(new { message = $"No existe turno con ID {id}" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
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
                ? Ok(new { message = "Turno eliminado correctamente" })
                : NotFound(new { message = $"No existe turno con ID {id}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
