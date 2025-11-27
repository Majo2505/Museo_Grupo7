using Museo.Models;
using Museo.Models.Dtos.City;
using Museo.Models.Dtos.Museum;
using Museo.Repositories;

namespace Museo.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cities;

        public CityService(ICityRepository cities)
        {
            _cities = cities;
        }

        public async Task<City> Create(CreateCityDto dto)
        {
            var exists = await _cities.ExistsByName(dto.Name);
            if (exists)
            {
                throw new InvalidOperationException("City name already exists");
            }

            var city = new City
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Country = dto.Country
            };
            await _cities.Add(city);
            return city;
        }

        public async Task<bool> Delete(Guid id)
        {
            await _cities.Delete(id);
            return true;
        }

        public async Task<IEnumerable<CityResponseDto>> GetAll()
        {
            var cities = await _cities.GetAll();

            return cities.Select(c => new CityResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country,

                Museum = c.Museum == null ? null : new MuseumSimpleDto
                {
                    Id = c.Museum.Id,
                    Name = c.Museum.Name,
                    Description = c.Museum.Description,
                    OpeningYear = c.Museum.OpeningYear,
                    CityId = c.Museum.CityId
                }
            });
        }
        public async Task<CityResponseDto?> GetById(Guid id)
        {
            var c = await _cities.GetById(id);

            if (c == null) return null;

            return new CityResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country,
                Museum = c.Museum == null ? null : new MuseumSimpleDto
                {
                    Id = c.Museum.Id,
                    Name = c.Museum.Name,
                    Description = c.Museum.Description,
                    OpeningYear = c.Museum.OpeningYear,
                    CityId = c.Museum.CityId
                }
            };
        }

        public async Task<City?> Update(Guid id, UpdateCityDto dto)
        {
            var city = await _cities.GetById(id);
            if (city == null) throw new InvalidOperationException("City ID not found");

            if (!city.Name.Equals(dto.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var exists = await _cities.ExistsByName(dto.Name);
                if (exists) throw new InvalidOperationException("City name already exists");
            }

            city.Name = dto.Name;
            city.Country = dto.Country;

            await _cities.Update(city);
            return city;
        }
    }
}
