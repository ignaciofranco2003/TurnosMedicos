namespace TurnosMedicos.Controllers.DTOS.Response
{

    public class TurnoResponseDto
    {
        public int Id { get; set; }
        public int? IdPaciente { get; set; }
        public int? IdMedico { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string? NombrePaciente { get; set; }
        public string? NombreMedico { get; set; }
    }

}