using Museo.Models;

namespace Museo.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetOne(Guid id);

        Task<IEnumerable<Comment>> GetByCanvasId(Guid canvasId);
        Task Add(Comment comment);
        Task Update(Comment comment);
        Task Delete(Comment comment);
    }
}