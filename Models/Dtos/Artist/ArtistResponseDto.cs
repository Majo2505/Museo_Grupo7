namespace Museo.Models.Dtos.Artist
{
    public class ArtistResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Specialty { get; set; }
        public string TypeOfWork { get; set; }
        public List<string> CanvasTitles { get; set; } = new();
    }
}
