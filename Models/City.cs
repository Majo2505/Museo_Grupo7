using System.ComponentModel.DataAnnotations;

namespace Museo.Models
{
    public class City
    {
        public Guid Id { get; set; }
        [Required, StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        [Required, StringLength(200)]   
        public string Pais { get; set; } = string.Empty;

        public Museum? Museum { get; set; }

        public Guid? MuseumId { get; set; }
    }
}

