using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services.Interfaces;

public interface IObraSocialService
{
    Task<List<ObraSocialResponseDto>> GetAllAsync();
    Task<ObraSocialResponseDto?> GetByIdAsync(int id);

    Task<ObraSocialResponseDto> CreateAsync(ObraSocialRequestDto dto);
    Task<bool> UpdateAsync(int id, ObraSocialRequestDto dto);

    Task<bool> DeleteAsync(int id);
}
