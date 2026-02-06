namespace TurnosMedicos.Entities.Enum
{
    public enum EstadoTurno
    {
        Pendiente,     // Turno creado pero no confirmado
        Confirmado,    // Confirmado por el paciente o el sistema
        Cancelado,     // Cancelado (paciente o clínica)
        Atendido,      // El paciente fue atendido
        Ausente        // No se presento
    }
}