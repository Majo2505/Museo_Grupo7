using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class Artista
    {
        public Guid Id { get; set; }
        [Required, StringLength(200)]
        public string Name { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; }
        [Required, StringLength(100)]
        public string Especialidad { get; set; }
        [Required, StringLength(100)]
        public string TipoDeObra { get; set; }
    }
}
