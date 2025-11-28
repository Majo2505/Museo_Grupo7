using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Museo.Models
{
    public class Canvas
    {
        public Guid Id { get; set; }
        [Required, StringLength(200)]
        public string Title { get; set; }
        [Required, StringLength(300)]
        public string Technique { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfEntry { get; set; } = DateTime.Now;


        public ICollection<Work> Works { get; set; } = new List<Work>();

        [JsonIgnore]
        public Museum? Museum { get; set; }

        public Guid MuseumId { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}