using Museo.Models;
using Museo.Models.Dtos.City;

namespace Museo.Services
{
    public interface ICityService
    {
        Task<IEnumerable<CityResponseDto>> GetAll();
        Task<CityResponseDto?> GetById(Guid id);
        Task<City> Create(CreateCityDto dto);
        Task<City?> Update(Guid id, UpdateCityDto dto);
        Task<bool> Delete(Guid id);
    }
}
