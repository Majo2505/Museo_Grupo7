using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos
{
    public record CreateCommentDto
    {
        [Required]
        [MinLength(10)]
        public required string Content { get; init; }
        [Required]
        public Guid CanvaId { get; init; }
    }
}
