using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos.City
{
    public class CreateCityDto
    {
        [Required, StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string Pais { get; set; } = string.Empty;
    }
}
