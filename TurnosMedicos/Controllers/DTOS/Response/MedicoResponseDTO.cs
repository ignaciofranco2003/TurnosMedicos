namespace TurnosMedicos.Controllers.DTOS.Response;

public class MedicoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int DuracionTurnoMin { get; set; }
}
