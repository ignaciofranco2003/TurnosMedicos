using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;
using TurnosMedicos.Services.Interfaces;

namespace TurnosMedicos.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Usuario")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _service;

        public PacienteController(IPacienteService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                if (items == null || !items.Any())
                    return Ok(new { success = true, message = "No se encontraron pacientes", data = items });

                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener pacientes", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                return item is null
                    ? NotFound(new { success = false, message = $"No existe paciente con ID {id}" })
                    : Ok(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener el paciente", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PacienteRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, message = "Paciente creado correctamente", data = created });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al crear el paciente", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PacienteRequestDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                return ok
                    ? Ok(new { success = true, message = "Paciente actualizado correctamente" })
                    : NotFound(new { success = false, message = $"No existe paciente con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al actualizar el paciente", error = ex.Message });
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
                    ? Ok(new { success = true, message = "Paciente eliminado correctamente" })
                    : NotFound(new { success = false, message = $"No existe paciente con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al eliminar el paciente", error = ex.Message });
            }
        }
    }
}
