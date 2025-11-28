using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos
{
    public record CommentResponseDto
    {
        public Guid Id { get; init; }
        public required string Content { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public Guid CanvasId { get; init; }
        public Guid UserId { get; init; }
        public string Username { get; init; } = string.Empty;
    }
}

