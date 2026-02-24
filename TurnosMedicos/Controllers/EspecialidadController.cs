using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Controllers
{

    [ApiController]
    [Route("api/especialidades")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class EspecialidadController : ControllerBase
    {
        private readonly IEspecialidadService _service;

        public EspecialidadController(IEspecialidadService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                if (items == null || !items.Any())
                    return Ok(new { success = true, message = "No se encontraron especialidades", data = items });

                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener especialidades", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                return item is null
                    ? NotFound(new { success = false, message = $"No existe especialidad con ID {id}" })
                    : Ok(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener la especialidad", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EspecialidadRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, message = "Especialidad creada correctamente", data = created });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al crear la especialidad", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EspecialidadRequestDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                return ok
                    ? Ok(new { success = true, message = "Especialidad actualizada correctamente" })
                    : NotFound(new { success = false, message = $"No existe especialidad con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al actualizar la especialidad", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                return ok
                    ? Ok(new { success = true, message = "Especialidad eliminada correctamente" })
                    : NotFound(new { success = false, message = $"No existe especialidad con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al eliminar la especialidad", error = ex.Message });
            }
        }
    }
}
