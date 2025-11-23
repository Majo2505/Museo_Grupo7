using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class Work
    {
        [Required]
        public Guid CanvasId { get; set; }
        [Required]
        public Guid ArtistId { get; set; }
        public Canvas? Canvas { get; set; }
        public Artist? Artist { get; set; }
    }
}