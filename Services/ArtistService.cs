using Museo.Models;
using Museo.Models.Dtos.Artist;
using Museo.Repositories;

namespace Museo.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artists;
        private readonly ICanvasRepository _canvas;

        public ArtistService(IArtistRepository artists, ICanvasRepository canvas)
        {
            _artists = artists;
            _canvas = canvas;
        }

        public async Task<Artist> Create(CreateArtistDto dto)
        {
            var exist = _artists.ExistsByName(dto.Name);
            if (exist.Result)
            {
                throw new InvalidOperationException("The name of the artist already exist");
            }
            var artista = new Artist
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Specialty = dto.Specialty,
                TypeOfWork = dto.TypeOfWork
            };
            await _artists.Add(artista);
            return artista;
        }

        public async Task<bool> Delete(Guid id)
        {
            await _artists.Delete(id);
            return true;
        }

        public async Task<IEnumerable<Artist>> GetAll()
        {
            return await _artists.GetAll();
        }

        public async Task<Artist?> GetById(Guid id)
        {
            return await _artists.GetById(id);
        }

        public async Task<Artist?> Update(Guid id, UpdateArtistDto artista)
        {
            throw new NotImplementedException();
        }
    }
}