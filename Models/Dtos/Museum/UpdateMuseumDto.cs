using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos.Museum
{
    public class UpdateMuseumDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; }
        [Range(0, 2025)]
        public int OpeningYear { get; set; }
        [Required]
        public Guid CityId { get; set; }
    }
}
