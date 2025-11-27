using Museo.Models.Dtos.Museum;

namespace Museo.Models.Dtos.City
{
    public class CityResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public MuseumSimpleDto? Museum { get; set; }
    }
}
