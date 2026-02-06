namespace TurnosMedicos.Entities
{
    public class Especialidad
    {
        public int Id { get; set; }
        public required string NombreEspecialidad { get; set; }

        public List<MedicoEspecialidad> MedicoEspecialidades { get; set; } = new();
        public byte[]? RowVersion { get; set; }

    }
}
