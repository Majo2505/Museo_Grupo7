namespace Museo.Models.Dtos.Canvas
{
    public class CanvasDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Technique { get; set; }
        public DateTime DateOfEntry { get; set; }
        public Guid MuseumId { get; set; }

        public List<string> Artists { get; set; } = new();

        public IEnumerable<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
    }
}
