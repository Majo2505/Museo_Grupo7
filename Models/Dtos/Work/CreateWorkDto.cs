using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos.Work
{
    public class CreateWorkDto
    {
        [Required]
        public Guid CanvasId { get; set; }
        [Required]
        public Guid ArtistId { get; set; }
    }
}
