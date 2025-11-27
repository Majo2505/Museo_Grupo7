using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Museo.Models
{
    public class Work
    {
        [Required]
        public Guid CanvasId { get; set; }
        [Required]
        public Guid ArtistId { get; set; }
        [JsonIgnore]
        public Canvas? Canvas { get; set; }
        [JsonIgnore]
        public Artist? Artist { get; set; }
    }
}