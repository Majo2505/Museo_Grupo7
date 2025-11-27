using Museo.Models;
using Museo.Models.Dtos.Work;

namespace Museo.Services
{
    public interface IWorkService
    {
        Task<IEnumerable<Work>> GetAll();
        Task<Work?> GetByRelation(Guid canvasId, Guid artistId);
        Task<Work> Create(CreateWorkDto dto);
        Task<bool> Delete(Guid canvasId, Guid artistId);
    }
}
