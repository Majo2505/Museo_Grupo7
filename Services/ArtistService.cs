using Museo.Models;
using Museo.Models.Dtos.Artist;
using Museo.Repositories;

namespace Museo.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artists;

        public ArtistService(IArtistRepository artists, ICanvasRepository canvas)
        {
            _artists = artists;
        }

        public async Task<Artist> Create(CreateArtistDto dto)
        {
            var exists = await _artists.ExistsByName(dto.Name);
            if (exists) throw new InvalidOperationException("The name of the artist already exists");
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

        public async Task<IEnumerable<ArtistResponseDto>> GetAll()
        {
            var artists = await _artists.GetAll();
            return artists.Select(a => new ArtistResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Specialty = a.Specialty,
                TypeOfWork = a.TypeOfWork,

                CanvasTitles = a.Works.Select(w => w.Canvas.Title).ToList()
            });
        }

        public async Task<ArtistResponseDto?> GetById(Guid id)
        {
            var artist = await _artists.GetById(id);

            if (artist == null) return null;

            return new ArtistResponseDto
            {
                Id = artist.Id,
                Name = artist.Name,
                Description = artist.Description,
                Specialty = artist.Specialty,
                TypeOfWork = artist.TypeOfWork,

                CanvasTitles = artist.Works.Select(w => w.Canvas.Title).ToList()
            };
        }


        public async Task<Artist?> Update(Guid id, UpdateArtistDto dto)
        {
            var artist = await _artists.GetById(id);
            if (artist == null) throw new InvalidOperationException("Artist ID not found");

            artist.Name = dto.Name;
            artist.Description = dto.Description;
            artist.Specialty = dto.Specialty;
            artist.TypeOfWork = dto.TypeOfWork;

            await _artists.Update(artist);
            return artist;
        }
    }
}