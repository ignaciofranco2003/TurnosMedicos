namespace TurnosMedicos.Controllers.DTOS.Request
{

    public class TurnoRequestDto
    {
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
    }

    public class TurnoUpdateRequestDto
    {
        public int? IdPaciente { get; set; }
        public int? IdMedico { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
    }

}