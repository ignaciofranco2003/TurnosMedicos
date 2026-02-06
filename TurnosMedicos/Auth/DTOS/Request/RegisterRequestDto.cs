namespace TurnosMedicos.Auth.DTOS.Request;

public class RegisterRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public TurnosMedicos.Auth.Entities.Role Role { get; set; } = TurnosMedicos.Auth.Entities.Role.Usuario; // default
}
