using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos
{
    public class UpdateCommentDto
    {
        [Required]
        [MinLength(10)]
        public required string Content { get; init; }
    }
}
