using TurnosMedicos.Data;
using TurnosMedicos.Entities;
using TurnosMedicos.Services.Interfaces;
using TurnosMedicos.Controllers.DTOS.Request;
using TurnosMedicos.Controllers.DTOS.Response;

namespace TurnosMedicos.Services;

public class ObraSocialService : CrudService<ObraSocial>, IObraSocialService
{
    public ObraSocialService(AppDbContext db) : base(db) { }

    private static ObraSocialResponseDto ToDto(ObraSocial o) => new()
    {
        Id = o.Id,
        Nombre = o.Nombre
    };

    public new async Task<List<ObraSocialResponseDto>> GetAllAsync()
    {
        var list = await base.GetAllAsync();
        return list.Select(ToDto).ToList();
    }

    public new async Task<ObraSocialResponseDto?> GetByIdAsync(int id)
    {
        var o = await base.GetByIdAsync(id);
        return o is null ? null : ToDto(o);
    }

    public async Task<ObraSocialResponseDto> CreateAsync(ObraSocialRequestDto dto)
    {
        var entity = new ObraSocial { Nombre = dto.Nombre };
        var created = await base.CreateAsync(entity);
        return ToDto(created);
    }

    public Task<bool> UpdateAsync(int id, ObraSocialRequestDto dto)
        => base.UpdateAsync(id, entity => entity.Nombre = dto.Nombre);

    public new Task<bool> DeleteAsync(int id) => base.DeleteAsync(id);
}
