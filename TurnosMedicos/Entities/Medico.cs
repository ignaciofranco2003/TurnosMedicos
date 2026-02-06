using TurnosMedicos.Entities.Values;
using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Entities
{
    public class Medico
    {
        public int Id { get; set; }
        [StringLength(100)]
        public required string Nombre { get; set; }
        [StringLength(10)]
        public required string DNI { get; set; }
        [StringLength(20)]
        public required string Telefono { get; set; }
        [StringLength(25)]
        public required string Matricula { get; set; }
        public int DuracionTurnoMin { get; set; }
        public List<MedicoEspecialidad> MedicoEspecialidades { get; set; } = new();
        public List<DisponibilidadMedico> Disponibilidades { get; set; } = new();
        public List<Turno> Turnos { get; set; } = new();
        public byte[]? RowVersion { get; set; }
    }
}
