namespace Museo.Models.Dtos.Museum
{
    public class MuseumSimpleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OpeningYear { get; set; }
        public Guid CityId { get; set; }
    }
}
