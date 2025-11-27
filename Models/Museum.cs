using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class Museum
    {
        public Guid Id { get; set; }
        [Required, StringLength(200)]
        public string Name { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; }
        [Range(0, 2025)]
        public int OpeningYear { get; set; }


        public ICollection<Canvas> Canvas { get; set; } = new List<Canvas>();

        public City? City { get; set; }

        public Guid CityId { get; set; }

    }
}
