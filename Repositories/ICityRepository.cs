using Museo.Models;

namespace Museo.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAll();
        Task<City?> GetById(Guid id);
        Task Add(City city);
        Task Delete(Guid id);
        Task Update(City city);
        Task<bool> ExistsByName(string name);
    }
}
