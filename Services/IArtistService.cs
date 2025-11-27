using Museo.Models;
using Museo.Models.Dtos.Artist;

namespace Museo.Services
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistResponseDto>> GetAll();
        Task<ArtistResponseDto?> GetById(Guid id);
        Task<Artist> Create(CreateArtistDto dto);
        Task<Artist?> Update(Guid id, UpdateArtistDto artista);
        Task<bool> Delete(Guid id);
    }
}