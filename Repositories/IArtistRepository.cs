using Museo.Data;
using Museo.Models;

namespace Museo.Repositories
{
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetAll();
        Task<Artist?> GetById(Guid id);
        Task Add(Artist artista);
        Task Delete(Guid id);
        Task Update(Artist artista);
        Task<bool> ExistsByName(string name);
    }
}
