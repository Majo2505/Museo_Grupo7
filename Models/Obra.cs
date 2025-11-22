using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class Obra
    {
        [Required]
        public Guid LienzoId { get; set; }
        [Required]
        public Guid ArtistaId { get; set; }
        public Lienzo? Lienzo { get; set; }
        public Artista? Artista { get; set; }
    }
}