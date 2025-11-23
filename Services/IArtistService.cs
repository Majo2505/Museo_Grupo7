using Museo.Models;
using Museo.Models.Dtos;

namespace Museo.Services
{
    public interface IArtistService
    {
        Task<IEnumerable<Artist>> GetAll();
        Task<Artist?> GetById(Guid id);
        Task<Artist> Create(CreateArtistDto dto);
        Task<Artist?> Update(Guid id, UpdateArtistDto artista);
        Task<bool> Delete(Guid id);
    }
}