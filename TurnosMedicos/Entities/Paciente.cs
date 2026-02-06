using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Entities
{
    public class Paciente
    {
        public int Id { get; set; }
        [StringLength(100)]
        public required string Nombre { get; set; }
        [StringLength(10)]
        public required string DNI { get; set; }
        [StringLength(20)]
        public required string Telefono { get; set; }
        public bool TieneObraSocial { get; set; }//Si tiene obra social
        public int? IdObraSocial{ get; set; } //Nombre de la obra social
        public ObraSocial? ObraSocial { get; set; }
        public List<Turno> Turnos { get; set; } = new();
        public byte[]? RowVersion { get; set; }

    }
}
