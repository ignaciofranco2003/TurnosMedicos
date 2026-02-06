namespace TurnosMedicos.Controllers.DTOS.Request;

public class PacienteRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool TieneObraSocial { get; set; }
    public int? IdObraSocial { get; set; }
}
