namespace TurnosMedicos.Entities.Values
{
    public class DisponibilidadMedico
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public required Medico Medico { get; set; }
        public DayOfWeek DiaSemana { get; set; }  // Lunes..Domingo
        public TimeOnly HoraDesde { get; set; }   // 08:00
        public TimeOnly HoraHasta { get; set; }   // 12:00
        public byte[]? RowVersion { get; set; }
    }

}
