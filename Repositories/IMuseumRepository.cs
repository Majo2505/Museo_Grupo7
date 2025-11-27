using Museo.Models;

namespace Museo.Repositories
{
    public interface IMuseumRepository
    {
        Task<IEnumerable<Museum>> GetAll();
        Task<Museum?> GetById(Guid id);
        Task Add(Museum museum);
        Task Delete(Guid id);
        Task Update(Museum museum);
        Task<bool> ExistsByName(string name);
    }
}
