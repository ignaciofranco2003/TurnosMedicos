using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services.Interfaces;

public interface IEspecialidadService
{
    Task<List<EspecialidadResponseDto>> GetAllAsync();
    Task<EspecialidadResponseDto?> GetByIdAsync(int id);

    Task<EspecialidadResponseDto> CreateAsync(EspecialidadRequestDto dto);
    Task<bool> UpdateAsync(int id, EspecialidadRequestDto dto);

    Task<bool> DeleteAsync(int id);
}
