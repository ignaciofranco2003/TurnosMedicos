namespace TurnosMedicos.Entities
{
    public class MedicoEspecialidad
    {
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; } = null!;
    }
}
