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
                var item = await _service.GetByIdAsync(id);
                return item is null ? NotFound() : Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MedicoRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
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
                    ? Ok(new { message = "Medico actualizado correctamente" })
                    : NotFound(new { message = $"No existe medico con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
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
                    : NotFound(new { message = $"No existe medico con ID {id}" });
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
                    ? Ok(new { message = "Medico eliminado correctamente" })
                    : NotFound(new { message = $"No existe medico con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
