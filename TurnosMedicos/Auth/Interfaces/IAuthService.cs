using TurnosMedicos.Auth.DTOS.Request;
using TurnosMedicos.Auth.DTOS.Response;

namespace TurnosMedicos.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task RegisterAsync(RegisterRequestDto dto);
}
