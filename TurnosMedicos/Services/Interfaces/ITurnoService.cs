using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services.Interfaces;

public interface ITurnoService
{
    Task<List<TurnoResponseDto>> GetAllAsync();
    Task<TurnoResponseDto?> GetByIdAsync(int id);

    Task<TurnoResponseDto> CreateAsync(TurnoRequestDto dto);
    Task<bool> UpdateAsync(int id, TurnoRequestDto dto);

    Task<bool> DeleteAsync(int id);

    IEnumerable<object> GetEstados();
}
