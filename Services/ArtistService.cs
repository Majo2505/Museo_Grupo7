using Museo.Models;
using Museo.Models.Dtos;
using Museo.Repositories;

namespace Museo.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistas;
        private readonly ICanvasRepository _lienzos;

        public ArtistService(IArtistRepository artistas, ICanvasRepository lienzos)
        {
            _artistas = artistas;
            _lienzos = lienzos;
        }

        public async Task<Artist> Create(CreateArtistDto dto)
        {
            var exist = _artistas.ExistsByName(dto.Name);
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
            await _artistas.Add(artista);
            return artista;
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artist>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Artist?> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Artist?> Update(Guid id, UpdateArtistDto artista)
        {
            throw new NotImplementedException();
        }
    }
}