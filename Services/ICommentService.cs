using Museo.Models.Dtos; 
using Museo.Models; 

namespace Museo.Services
{
    public interface ICommentService
    {

        Task<IEnumerable<CommentResponseDto>> GetCommentsByCanvas(Guid canvasId);

        Task<CommentResponseDto> CreateComment(CreateCommentDto dto, Guid currentUserId);

        Task<CommentResponseDto> UpdateComment(Guid commentId, UpdateCommentDto dto, Guid currentUserId);

        Task DeleteComment(Guid commentId, Guid currentUserId);
    }
}
