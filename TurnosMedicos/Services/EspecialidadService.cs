using TurnosMedicos.Data;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services;

public class EspecialidadService : CrudService<Especialidad>, IEspecialidadService
{
    public EspecialidadService(AppDbContext db) : base(db) { }

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
        var entity = new Especialidad { NombreEspecialidad = dto.NombreEspecialidad };
        var created = await base.CreateAsync(entity);
        return ToDto(created);
    }

    public Task<bool> UpdateAsync(int id, EspecialidadRequestDto dto)
        => base.UpdateAsync(id, entity => entity.NombreEspecialidad = dto.NombreEspecialidad);

    public new Task<bool> DeleteAsync(int id) => base.DeleteAsync(id);
}
