using Museo.Models;

namespace Museo.Repositories
{
    public interface ICanvasRepository
    {
        Task<IEnumerable<Canvas>> GetAll();
        Task<Canvas?> GetById(Guid id);
        Task Add(Canvas artista);
        Task Delete(Guid id);
        Task Update(Canvas artista);
    }
}
