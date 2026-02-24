using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;
using TurnosMedicos.Services.Interfaces;

namespace TurnosMedicos.Controllers
{
    [ApiController]
    [Route("api/medicos")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _service;

        public MedicoController(IMedicoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                if (items == null || !items.Any())
                    return Ok(new { success = true, message = "No se encontraron médicos", data = items });

                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener médicos", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                return item is null
                    ? NotFound(new { success = false, message = $"No existe medico con ID {id}" })
                    : Ok(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener el médico", error = ex.Message });
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MedicoRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, message = "Médico creado correctamente", data = created });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al crear el médico", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicoRequestDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                return ok
                    ? Ok(new { success = true, message = "Médico actualizado correctamente" })
                    : NotFound(new { success = false, message = $"No existe medico con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al actualizar el médico", error = ex.Message });
            }
        }

        [HttpPost("{id:int}/especialidades")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        [Obsolete("Enviar especialidades en el mismo body de Create/Update (MedicoRequestDto.EspecialidadesNombres).")]
        public async Task<IActionResult> SetEspecialidades(int id, [FromBody] AsignarEspecialidadesRequestDto dto)
        {
            try
            {
                var ok = await _service.SetEspecialidadesAsync(id, dto.EspecialidadesNombres);
                return ok
                    ? NoContent()
                    : NotFound(new { success = false, message = $"No existe medico con ID {id}" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al asignar especialidades", error = ex.Message });
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
                    ? Ok(new { success = true, message = "Médico eliminado correctamente" })
                    : NotFound(new { success = false, message = $"No existe medico con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al eliminar el médico", error = ex.Message });
            }
        }
    }
}
