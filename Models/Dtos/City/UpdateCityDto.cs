using System.ComponentModel.DataAnnotations;

namespace Museo.Models.Dtos.City
{
    public class UpdateCityDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string Country { get; set; } = string.Empty;
    }
}
