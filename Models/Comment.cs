using Museo.Models;

namespace Museo.Models
{
    public class Comentario
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid CanvaId { get; set; }
        public Canvas Canva { get; set; } = default!;
    }
}

