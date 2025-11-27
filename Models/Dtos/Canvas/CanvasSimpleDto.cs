namespace Museo.Models.Dtos.Canvas
{
    public class CanvasSimpleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Technique { get; set; }
        public DateTime DateOfEntry { get; set; }
        public List<string> ArtistNames { get; set; } = new();
    }
}
