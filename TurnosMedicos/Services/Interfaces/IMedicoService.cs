using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services.Interfaces;

public interface IMedicoService
{
    Task<List<MedicoResponseDto>> GetAllAsync();
    Task<MedicoResponseDto?> GetByIdAsync(int id);

    Task<MedicoResponseDto> CreateAsync(MedicoRequestDto dto);
    Task<bool> UpdateAsync(int id, MedicoRequestDto dto);

    Task<bool> SetEspecialidadesAsync(int medicoId, List<string> especialidadesNombres);

    Task<bool> DeleteAsync(int id);
}
