using System.ComponentModel.DataAnnotations;


namespace Museo.Models.Dtos
{
    public record UpdateCommentDto
    {
        [Required]
        [MinLength(10)]
        public required string Content { get; init; }

    }

}