using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class Artist
    {
        public Guid Id { get; set; }
        [Required, StringLength(200)]
        public string Name { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string Specialty { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string TypeOfWork { get; set; }

        public ICollection<Work> Works { get; set; } = new List<Work>();
    }
}