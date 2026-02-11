namespace TurnosMedicos.Controllers.DTOS.Request;

public class AsignarEspecialidadesRequestDto
{
    // Lista de nombres de especialidades a dejar asignadas al médico.
    // (Se reemplazan las existentes por esta lista)
    // El front enviará una lista con uno o varios nombres.
    public List<string> EspecialidadesNombres { get; set; } = new();
}
