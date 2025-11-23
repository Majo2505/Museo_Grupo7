using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos
{
    public class UpdateCanvasDto
    {
        [Required, StringLength(200)]
        public string Title { get; set; }
        [Required, StringLength(300)]
        public string Technique { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfEntry { get; set; } = DateTime.Now;
    }
}
