using Museo.Models;
using Museo.Models.Dtos.Canvas;
using Museo.Models.Dtos.Museum;
using Museo.Repositories;

namespace Museo.Services
{
    public class MuseumService : IMuseumService
    {
        private readonly IMuseumRepository _museums;
        private readonly ICityRepository _cities;

        public MuseumService(IMuseumRepository museums, ICityRepository cities)
        {
            _museums = museums;
            _cities = cities;
        }

        public async Task<Museum> Create(CreateMuseumDto dto)
        {
            var city = await _cities.GetById(dto.CityId);
            if (city == null)
            {
                throw new InvalidOperationException("City ID not found");
            }

            var exists = await _museums.ExistsByName(dto.Name);
            if (exists)
            {
                throw new InvalidOperationException("Museum name already exists");
            }

            var museum = new Museum
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                OpeningYear = dto.OpeningYear,
                CityId = dto.CityId
            };
            await _museums.Add(museum);
            return museum;
        }

        public async Task<bool> Delete(Guid id)
        {
            await _museums.Delete(id);
            return true;
        }

        public async Task<IEnumerable<MuseumResponseDto>> GetAll()
        {
            var museums = await _museums.GetAll();

            return museums.Select(m => new MuseumResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                OpeningYear = m.OpeningYear,
                CityId = m.CityId,
                Canvas = m.Canvas.Select(c => new CanvasSimpleDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Technique = c.Technique,
                    DateOfEntry = c.DateOfEntry,
                    ArtistNames = c.Works.Select(w => w.Artist.Name).ToList()
                }).ToList()
            });
        }

        public async Task<MuseumResponseDto?> GetById(Guid id)
        {

            var m = await _museums.GetById(id);

            if (m == null) return null;

            return new MuseumResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                OpeningYear = m.OpeningYear,
                CityId = m.CityId,
                Canvas = m.Canvas.Select(c => new CanvasSimpleDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Technique = c.Technique,
                    DateOfEntry = c.DateOfEntry,
                    ArtistNames = c.Works.Select(w => w.Artist.Name).ToList()
                }).ToList()
            };
        }

        public async Task<Museum?> Update(Guid id, UpdateMuseumDto dto)
        {
            var museum = await _museums.GetById(id);
            if (museum == null) throw new InvalidOperationException("Museum ID not found");

            if (museum.CityId != dto.CityId)
            {
                var city = await _cities.GetById(dto.CityId);
                if (city == null) throw new InvalidOperationException("New City ID not found");
                museum.CityId = dto.CityId;
            }

            museum.Name = dto.Name;
            museum.Description = dto.Description;
            museum.OpeningYear = dto.OpeningYear;

            await _museums.Update(museum);
            return museum;
        }
    }
}
