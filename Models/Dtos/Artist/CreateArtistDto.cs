using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos.Artist
{
    public class CreateArtistDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string Specialty { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string TypeOfWork { get; set; }
    }
}
