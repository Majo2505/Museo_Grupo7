using Museo.Models;

namespace Museo.Repositories
{
    public interface IWorkRepository
    {
        Task<IEnumerable<Work>> GetAll();
        Task<Work?> GetByIds(Guid canvasId, Guid artistId);
        Task Add(Work work);
        Task Delete(Guid canvasId, Guid artistId);
        Task Update(Work work);
    }
}
