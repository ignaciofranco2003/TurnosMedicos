namespace TurnosMedicos.Auth.DTOS.Request;

public class LoginRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
