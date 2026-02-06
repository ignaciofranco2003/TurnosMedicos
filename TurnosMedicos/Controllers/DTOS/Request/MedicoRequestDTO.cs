namespace TurnosMedicos.Controllers.DTOS.Request;

public class MedicoRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int DuracionTurnoMin { get; set; }
}
