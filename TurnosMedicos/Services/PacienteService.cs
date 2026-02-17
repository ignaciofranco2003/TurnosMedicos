using TurnosMedicos.Data;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services;

public class PacienteService : CrudService<Paciente>, IPacienteService
{
    public PacienteService(AppDbContext db) : base(db) { }

    private static PacienteResponseDto ToDto(Paciente p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        DNI = p.DNI,
        Telefono = p.Telefono,
        TieneObraSocial = p.TieneObraSocial,
        IdObraSocial = p.IdObraSocial
    };

    public new async Task<List<PacienteResponseDto>> GetAllAsync()
    {
        var list = await base.GetAllAsync();
        return list.Select(ToDto).ToList();
    }

    public new async Task<PacienteResponseDto?> GetByIdAsync(int id)
    {
        var p = await base.GetByIdAsync(id);
        return p is null ? null : ToDto(p);
    }

    public async Task<PacienteResponseDto> CreateAsync(PacienteRequestDto dto)
    {
        var entity = new Paciente
        {
            Nombre = dto.Nombre,
            DNI = dto.DNI,
            Telefono = dto.Telefono,
            TieneObraSocial = dto.TieneObraSocial,
            IdObraSocial = dto.IdObraSocial
        };

        var created = await base.CreateAsync(entity);
        return ToDto(created);
    }

    public Task<bool> UpdateAsync(int id, PacienteRequestDto dto)
        => base.UpdateAsync(id, entity =>
        {
            entity.Nombre = dto.Nombre;
            entity.DNI = dto.DNI;
            entity.Telefono = dto.Telefono;
            entity.TieneObraSocial = dto.TieneObraSocial;
            entity.IdObraSocial = dto.IdObraSocial;
        });

    public new async Task<bool> DeleteAsync(int id)
    {
        var paciente = await _db.Pacientes
            .Include(p => p.Turnos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (paciente is null) return false;

        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var nombre = paciente.Nombre;
            var telefono = paciente.Telefono;
            foreach (var t in paciente.Turnos)
            {
                var prefix = string.IsNullOrWhiteSpace(t.Observaciones) ? string.Empty : "\n";
                t.Observaciones = (t.Observaciones ?? string.Empty) + prefix + $"[sistema]:\"Paciente {nombre} ({telefono}) eliminado\"";
            }

            await _db.SaveChangesAsync();

            _db.Pacientes.Remove(paciente);
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
