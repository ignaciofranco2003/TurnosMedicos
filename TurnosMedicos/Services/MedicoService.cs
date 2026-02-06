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
        DuracionTurnoMin = m.DuracionTurnoMin
    };

    public new async Task<List<MedicoResponseDto>> GetAllAsync()
    {
        var list = await base.GetAllAsync();
        return list.Select(ToDto).ToList();
    }

    public new async Task<MedicoResponseDto?> GetByIdAsync(int id)
    {
        var m = await base.GetByIdAsync(id);
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
        return ToDto(created);
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
