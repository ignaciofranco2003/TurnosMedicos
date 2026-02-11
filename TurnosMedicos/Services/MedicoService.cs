using TurnosMedicos.Data;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;
using Microsoft.EntityFrameworkCore;

namespace TurnosMedicos.Services;

public class MedicoService : CrudService<Medico>, IMedicoService
{
    public MedicoService(AppDbContext db) : base(db) { }

    private static MedicoResponseDto ToDto(Medico m) => new()
    {
        Id = m.Id,
        Nombre = m.Nombre,
        DNI = m.DNI,
        Telefono = m.Telefono,
        Matricula = m.Matricula,
        DuracionTurnoMin = m.DuracionTurnoMin,
        Especialidades = m.MedicoEspecialidades
            .Select(me => me.Especialidad)
            .Where(e => e is not null)
            .Select(e => new EspecialidadResponseDto
            {
                Id = e.Id,
                NombreEspecialidad = e.NombreEspecialidad
            })
            .ToList()
    };

    // IMPORTANTE: acá sí incluimos la relación N..N
    public new async Task<List<MedicoResponseDto>> GetAllAsync()
    {
        var list = await _db.Medicos
            .Include(m => m.MedicoEspecialidades)
                .ThenInclude(me => me.Especialidad)
            .AsNoTracking()
            .ToListAsync();

        return list.Select(ToDto).ToList();
    }

    // IMPORTANTE: acá sí incluimos la relación N..N
    public new async Task<MedicoResponseDto?> GetByIdAsync(int id)
    {
        var m = await _db.Medicos
            .Include(x => x.MedicoEspecialidades)
                .ThenInclude(me => me.Especialidad)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return m is null ? null : ToDto(m);
    }

    public async Task<MedicoResponseDto> CreateAsync(MedicoRequestDto dto)
    {
        var entity = new Medico
        {
            Nombre = dto.Nombre,
            DNI = dto.DNI,
            Telefono = dto.Telefono,
            Matricula = dto.Matricula,
            DuracionTurnoMin = dto.DuracionTurnoMin
        };

        var created = await base.CreateAsync(entity);

        // Devolver consistente (con especialidades, aunque sea lista vacía)
        var createdFull = await _db.Medicos
            .Include(m => m.MedicoEspecialidades)
                .ThenInclude(me => me.Especialidad)
            .AsNoTracking()
            .FirstAsync(m => m.Id == created.Id);

        return ToDto(createdFull);
    }

    public Task<bool> UpdateAsync(int id, MedicoRequestDto dto)
        => base.UpdateAsync(id, entity =>
        {
            entity.Nombre = dto.Nombre;
            entity.DNI = dto.DNI;
            entity.Telefono = dto.Telefono;
            entity.Matricula = dto.Matricula;
            entity.DuracionTurnoMin = dto.DuracionTurnoMin;
        });

    public async Task<bool> SetEspecialidadesAsync(int medicoId, List<string> especialidadesNombres)
    {
        // Normalizar: lista no nula, sin duplicados, sin nombres vacíos
        var nombres = (especialidadesNombres ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var medico = await _db.Medicos
            .Include(m => m.MedicoEspecialidades)
            .FirstOrDefaultAsync(m => m.Id == medicoId);

        if (medico is null) return false;

        // Buscar especialidades por nombre (case-insensitive)
        var nombresLower = nombres.Select(n => n.ToLower()).ToList();
        var existentes = await _db.Especialidades
            .Where(e => nombresLower.Contains(e.NombreEspecialidad.ToLower()))
            .Select(e => new { e.Id, e.NombreEspecialidad })
            .ToListAsync();

        // Validar faltantes (comparar en case-insensitive)
        var existentesNombres = existentes
            .Select(e => e.NombreEspecialidad)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var faltantes = nombres
            .Where(n => !existentesNombres.Contains(n))
            .ToList();

        if (faltantes.Count > 0)
            throw new ArgumentException($"Especialidad(es) inexistente(s): {string.Join(", ", faltantes)}");

        // Reemplazar vínculos
        medico.MedicoEspecialidades.Clear();

        foreach (var esp in existentes)
        {
            medico.MedicoEspecialidades.Add(new MedicoEspecialidad
            {
                MedicoId = medicoId,
                EspecialidadId = esp.Id
            });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public new async Task<bool> DeleteAsync(int id)
    {
        var medico = await _db.Medicos
            .Include(m => m.Turnos)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medico is null) return false;

        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var nombre = medico.Nombre;
            foreach (var t in medico.Turnos)
            {
                t.Observaciones = (t.Observaciones ?? string.Empty) + $" [sistema]:\"Medico {nombre} eliminado\"";
            }

            await _db.SaveChangesAsync();

            _db.Medicos.Remove(medico);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();
            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
