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

    private static List<string> NormalizeNombres(List<string>? especialidadesNombres)
        => (especialidadesNombres ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

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

    public new async Task<List<MedicoResponseDto>> GetAllAsync()
    {
        var list = await _db.Medicos
            .Include(m => m.MedicoEspecialidades)
                .ThenInclude(me => me.Especialidad)
            .AsNoTracking()
            .ToListAsync();

        return list.Select(ToDto).ToList();
    }

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
        using var tx = await _db.Database.BeginTransactionAsync();
        var entity = new Medico
        {
            Nombre = dto.Nombre,
            DNI = dto.DNI,
            Telefono = dto.Telefono,
            Matricula = dto.Matricula,
            DuracionTurnoMin = dto.DuracionTurnoMin
        };

        var created = await base.CreateAsync(entity);

        var nombres = NormalizeNombres(dto.EspecialidadesNombres);
        if (nombres.Count > 0)
        {
            await SetEspecialidadesAsync(created.Id, nombres);
        }

        await tx.CommitAsync();

        var createdFull = await _db.Medicos
            .Include(m => m.MedicoEspecialidades)
                .ThenInclude(me => me.Especialidad)
            .AsNoTracking()
            .FirstAsync(m => m.Id == created.Id);

        return ToDto(createdFull);
    }

    public async Task<bool> UpdateAsync(int id, MedicoRequestDto dto)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        var ok = await base.UpdateAsync(id, entity =>
        {
            entity.Nombre = dto.Nombre;
            entity.DNI = dto.DNI;
            entity.Telefono = dto.Telefono;
            entity.Matricula = dto.Matricula;
            entity.DuracionTurnoMin = dto.DuracionTurnoMin;
        });

        if (!ok) return false;

        var nombres = NormalizeNombres(dto.EspecialidadesNombres);
        await SetEspecialidadesAsync(id, nombres);

        await tx.CommitAsync();
        return true;
    }

    public async Task<bool> SetEspecialidadesAsync(int medicoId, List<string> especialidadesNombres)
    {
        var nombres = NormalizeNombres(especialidadesNombres);

        var medico = await _db.Medicos
            .Include(m => m.MedicoEspecialidades)
            .FirstOrDefaultAsync(m => m.Id == medicoId);

        if (medico is null) return false;

        var nombresLower = nombres.Select(n => n.ToLower()).ToList();
        var existentes = await _db.Especialidades
            .Where(e => nombresLower.Contains(e.NombreEspecialidad.ToLower()))
            .Select(e => new { e.Id, e.NombreEspecialidad })
            .ToListAsync();

        var existentesNombres = existentes
            .Select(e => e.NombreEspecialidad)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var faltantes = nombres
            .Where(n => !existentesNombres.Contains(n))
            .ToList();

        if (faltantes.Count > 0)
            throw new ArgumentException($"Especialidad(es) inexistente(s): {string.Join(", ", faltantes)}");

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
            var matricula = medico.Matricula;
            foreach (var t in medico.Turnos)
            {
                var prefix = string.IsNullOrWhiteSpace(t.Observaciones) ? string.Empty : "\n";
                t.Observaciones = (t.Observaciones ?? string.Empty) + prefix + $"[sistema]:\"Medico matrícula {matricula} eliminado\"";
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
