using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Controllers
{
    [ApiController]
    [Route("api/obrassociales")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class ObraSocialController : ControllerBase
    {
        private readonly IObraSocialService _service;

        public ObraSocialController(IObraSocialService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                if (items == null || !items.Any())
                    return Ok(new { success = true, message = "No se encontraron obras sociales", data = items });

                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener obras sociales", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                return item is null
                    ? NotFound(new { success = false, message = $"No existe obra social con ID {id}" })
                    : Ok(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al obtener la obra social", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ObraSocialRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, message = "Obra social creada correctamente", data = created });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al crear la obra social", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ObraSocialRequestDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                return ok
                    ? Ok(new { success = true, message = "Obra social actualizada correctamente" })
                    : NotFound(new { success = false, message = $"No existe obra social con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al actualizar la obra social", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                return ok
                    ? Ok(new { success = true, message = "Obra social eliminada correctamente" })
                    : NotFound(new { success = false, message = $"No existe obra social con ID {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error al eliminar la obra social", error = ex.Message });
            }
        }
    }
}
