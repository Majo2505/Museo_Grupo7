using Museo.Models;
using Museo.Models.Dtos;
using Museo.Repositories;

namespace Museo.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;

        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<IEnumerable<CommentResponseDto>> GetCommentsByCanvas(Guid canvasId)
        {
            var comments = await _repository.GetByCanvasId(canvasId);

            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CanvasId = c.CanvasId,
                UserId = c.UserId,
                Username = c.User?.Username ?? "Unknown"
            });
        }


        public async Task<CommentResponseDto> CreateComment(CreateCommentDto dto, Guid currentUserId)
        {
    
            var newComment = new Comment
            {
                Id = Guid.NewGuid(), 
                Content = dto.Content,
                CanvasId = dto.CanvasId,
                UserId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.Add(newComment);

            var createdComment = await _repository.GetOne(newComment.Id);

            if (createdComment == null) throw new Exception("Error creating comment");

            return new CommentResponseDto
            {
                Id = createdComment.Id,
                Content = createdComment.Content,
                CreatedAt = createdComment.CreatedAt,
                CanvasId = createdComment.CanvasId,
                UserId = createdComment.UserId,
                Username = createdComment.User?.Username ?? "Unknown"
            };
        }

        public async Task<CommentResponseDto> UpdateComment(Guid id, UpdateCommentDto dto, Guid currentUserId)
        {
            var comment = await _repository.GetOne(id);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment not found.");
            }

            if (comment.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this comment.");
            }

            comment.Content = dto.Content;
            await _repository.Update(comment);

            return new CommentResponseDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CanvasId = comment.CanvasId,
                UserId = comment.UserId,
                Username = comment.User?.Username ?? "Unknown"
            };
        }

        public async Task DeleteComment(Guid id, Guid currentUserId)
        {
            var comment = await _repository.GetOne(id);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment not found.");
            }

            if (comment.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this comment.");
            }

            await _repository.Delete(comment);
        }


    }
}
