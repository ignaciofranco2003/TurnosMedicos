using TurnosMedicos.Entities.Enum;
using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Entities
{
    public class Turno
    {
        public int Id { get; set; }
        public int? IdPaciente{ get; set; }
        public int? IdMedico { get; set; }
        public required DateTime Inicio { get; set; }
        public required DateTime Fin { get; set; }   // Inicio + duración
        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;
        [StringLength(200)]
        public string? Observaciones { get; set; }

        public Paciente? Paciente { get; set; }
        public Medico? Medico { get; set; }
        public byte[]? RowVersion { get; set; }

    }
}
