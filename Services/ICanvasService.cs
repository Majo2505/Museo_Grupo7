using Museo.Models;
using Museo.Models.Dtos.Canvas;

namespace Museo.Services
{
    public interface ICanvasService
    {
        Task<IEnumerable<CanvasDto>> GetAll();
        Task<CanvasDto?> GetById(Guid id);
        Task<Canvas> Create(CreateCanvasDto dto);
        Task<Canvas?> Update(Guid id, UpdateCanvasDto dto);
        Task<bool> Delete(Guid id);
    }
}
