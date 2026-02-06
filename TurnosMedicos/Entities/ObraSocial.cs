using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Entities
{
    public class ObraSocial
    {
        public int Id { get; set; }
        [StringLength(20)]
        public required string Nombre { get; set; }

        public List<Paciente> Pacientes { get; set; } = new();
        public byte[]? RowVersion { get; set; }
    }
}
