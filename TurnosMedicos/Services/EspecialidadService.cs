using TurnosMedicos.Data;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;
using Microsoft.EntityFrameworkCore;

namespace TurnosMedicos.Services;

public class EspecialidadService : CrudService<Especialidad>, IEspecialidadService
{
    public EspecialidadService(AppDbContext db) : base(db) { }

    private static string NormalizeNombre(string nombre)
        => (nombre ?? string.Empty).Trim();

    private async Task EnsureNombreUniqueAsync(string nombreNormalizado, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(nombreNormalizado))
            throw new ArgumentException("El nombre de la especialidad es requerido");

        var query = _db.Especialidades.AsNoTracking();
        if (excludeId is not null)
            query = query.Where(e => e.Id != excludeId.Value);

        var exists = await query
            .AnyAsync(e => e.NombreEspecialidad.ToLower() == nombreNormalizado.ToLower());

        if (exists)
            throw new ArgumentException($"Ya existe una especialidad con nombre '{nombreNormalizado}'");
    }

    private static EspecialidadResponseDto ToDto(Especialidad e) => new()
    {
        Id = e.Id,
        NombreEspecialidad = e.NombreEspecialidad
    };

    public new async Task<List<EspecialidadResponseDto>> GetAllAsync()
    {
        var list = await base.GetAllAsync();
        return list.Select(ToDto).ToList();
    }

    public new async Task<EspecialidadResponseDto?> GetByIdAsync(int id)
    {
        var e = await base.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<EspecialidadResponseDto> CreateAsync(EspecialidadRequestDto dto)
    {
        var nombre = NormalizeNombre(dto.NombreEspecialidad);
        await EnsureNombreUniqueAsync(nombre);

        var entity = new Especialidad { NombreEspecialidad = nombre };
        var created = await base.CreateAsync(entity);
        return ToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, EspecialidadRequestDto dto)
    {
        var nombre = NormalizeNombre(dto.NombreEspecialidad);
        await EnsureNombreUniqueAsync(nombre, excludeId: id);

        return await base.UpdateAsync(id, entity => entity.NombreEspecialidad = nombre);
    }

    public new Task<bool> DeleteAsync(int id) => base.DeleteAsync(id);
}
