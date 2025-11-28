using Museo.Models;
using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        [Required, StringLength(500)] 
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relation N:1 with User
        public Guid UserId { get; set; }
        public User User { get; set; } = default!; 

        // Relation N:1 wirh Canvas
        public Guid CanvasId { get; set; } 
        public Canvas Canvas { get; set; } = default!;
    }
}