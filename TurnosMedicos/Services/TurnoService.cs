using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;
using TurnosMedicos.Data;
using TurnosMedicos.Entities;
using TurnosMedicos.Entities.Enum;
using TurnosMedicos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TurnosMedicos.Services;

public class TurnoService : CrudService<Turno>, ITurnoService
{
    public TurnoService(AppDbContext db) : base(db) { }

    // --- helper ---
    private static bool TryParseEstado(string? estadoStr, out EstadoTurno estado)
    {
        if (string.IsNullOrWhiteSpace(estadoStr))
        {
            estado = EstadoTurno.Pendiente;
            return true;
        }

        return System.Enum.TryParse(estadoStr.Trim(), ignoreCase: true, out estado);
    }

    private static TurnoResponseDto ToDto(Turno t) => new()
    {
        Id = t.Id,
        IdPaciente = t.IdPaciente,
        IdMedico = t.IdMedico,
        Inicio = t.Inicio,
        Fin = t.Fin,
        Estado = t.Estado.ToString(),
        Observaciones = t.Observaciones,
        NombrePaciente = t.Paciente?.Nombre,
        NombreMedico = t.Medico?.Nombre
    };

    public new async Task<List<TurnoResponseDto>> GetAllAsync()
    {
        var list = await _db.Turnos
            .AsNoTracking()
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .ToListAsync();

        return list.Select(ToDto).ToList();
    }

    public new async Task<TurnoResponseDto?> GetByIdAsync(int id)
    {
        var t = await _db.Turnos
            .AsNoTracking()
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .FirstOrDefaultAsync(t => t.Id == id);

        return t is null ? null : ToDto(t);
    }

    public async Task<TurnoResponseDto> CreateAsync(TurnoRequestDto dto)
    {
        if (!TryParseEstado(dto.Estado, out var estado))
            throw new ArgumentException("Estado inválido");

        // Check overlap for medico
        await EnsureNoOverlapAsync(null, dto.IdMedico, dto.Inicio, dto.Fin);

        var entity = new Turno
        {
            IdPaciente = dto.IdPaciente,
            IdMedico = dto.IdMedico,
            Inicio = dto.Inicio,
            Fin = dto.Fin,
            Estado = estado,
            Observaciones = dto.Observaciones
        };

        var created = await base.CreateAsync(entity);
        return ToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, TurnoRequestDto dto)
    {
        if (!TryParseEstado(dto.Estado, out var estado))
            throw new ArgumentException("Estado inválido");

        // Ensure no overlap excluding current turno
        await EnsureNoOverlapAsync(id, dto.IdMedico, dto.Inicio, dto.Fin);

        return await base.UpdateAsync(id, turno =>
        {
            turno.IdPaciente = dto.IdPaciente;
            turno.IdMedico = dto.IdMedico;
            turno.Inicio = dto.Inicio;
            turno.Fin = dto.Fin;
            turno.Estado = estado;
            turno.Observaciones = dto.Observaciones;
        });
    }

    private async Task EnsureNoOverlapAsync(int? excludeTurnoId, int medicoId, DateTime inicio, DateTime fin)
    {
        var overlaps = await _db.Turnos
            .Where(t => t.IdMedico == medicoId && (excludeTurnoId == null || t.Id != excludeTurnoId))
            .Where(t => !(t.Fin <= inicio || t.Inicio >= fin))
            .AnyAsync();

        if (overlaps)
            throw new ArgumentException("El médico tiene un turno que se solapa en ese horario");
    }

    public IEnumerable<object> GetEstados()
        => System.Enum.GetValues<EstadoTurno>()
            .Select(e => new { id = (int)e, nombre = e.ToString() });

    public new async Task<bool> DeleteAsync(int id)
    {
        // Additional logic for cascading updates on Patient/Medico deletion will be added here
        return await base.DeleteAsync(id);
    }
}
