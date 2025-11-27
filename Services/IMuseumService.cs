using Museo.Models;
using Museo.Models.Dtos.Museum;

namespace Museo.Services
{
    public interface IMuseumService
    {
        Task<IEnumerable<MuseumResponseDto>> GetAll();
        Task<MuseumResponseDto?> GetById(Guid id);
        Task<Museum> Create(CreateMuseumDto dto);
        Task<Museum?> Update(Guid id, UpdateMuseumDto dto);
        Task<bool> Delete(Guid id);
    }
}
