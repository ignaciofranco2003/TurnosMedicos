using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services.Interfaces;

public interface IPacienteService
{
    Task<List<PacienteResponseDto>> GetAllAsync();
    Task<PacienteResponseDto?> GetByIdAsync(int id);

    Task<PacienteResponseDto> CreateAsync(PacienteRequestDto dto);
    Task<bool> UpdateAsync(int id, PacienteRequestDto dto);

    Task<bool> DeleteAsync(int id);
}
