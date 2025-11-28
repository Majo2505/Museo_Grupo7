using Museo.Models;
using Museo.Models.Dtos;
using Museo.Models.Dtos.Canvas;
using Museo.Repositories;

namespace Museo.Services
{
    public class CanvasService : ICanvasService
    {
        private readonly ICanvasRepository _canvasRepo;
        private readonly IMuseumRepository _museumRepo;
        private readonly IArtistRepository _artistRepo;
        private readonly IWorkRepository _workRepo;

        public CanvasService(
            ICanvasRepository canvasRepo,
            IMuseumRepository museumRepo,
            IArtistRepository artistRepo,
            IWorkRepository workRepo)
        {
            _canvasRepo = canvasRepo;
            _museumRepo = museumRepo;
            _artistRepo = artistRepo;
            _workRepo = workRepo;
        }

        public async Task<Canvas> Create(CreateCanvasDto dto)
        {
            var museum = await _museumRepo.GetById(dto.MuseumId);
            if (museum == null)
            {
                throw new InvalidOperationException("Museum with the specified ID was not found");
            }

            foreach (var artistId in dto.ArtistIds.Distinct())
            {
                var artist = await _artistRepo.GetById(artistId);
                if (artist == null)
                {
                    throw new InvalidOperationException($"Artist with ID {artistId} not found");
                }
            }

            var canvas = new Canvas
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Technique = dto.Technique,
                DateOfEntry = dto.DateOfEntry,
                MuseumId = dto.MuseumId
            };
            await _canvasRepo.Add(canvas);

            foreach (var artistId in dto.ArtistIds.Distinct())
            {
                var work = new Work
                {
                    CanvasId = canvas.Id,
                    ArtistId = artistId
                };
                await _workRepo.Add(work);
            }
            return canvas;
        }

        public async Task<bool> Delete(Guid id)
        {
            await _canvasRepo.Delete(id);
            return true;
        }

        public async Task<IEnumerable<CanvasDto>> GetAll() 
        {
            var canvases = await _canvasRepo.GetAll();
            return canvases.Select(canva => new CanvasDto
            {
                Id = canva.Id,
                Title = canva.Title,
                Technique = canva.Technique,
                DateOfEntry = canva.DateOfEntry,
                MuseumId = canva.MuseumId,

                Artists = canva.Works.Select(w => w.Artist.Name).ToList()
            });
        }

        public async Task<CanvasDto?> GetById(Guid id)
        {
            var canva = await _canvasRepo.GetById(id);

            if (canva == null)
            {
                return null;
            }

            var Dto = new CanvasDto
            {
                Id = canva.Id,
                Title = canva.Title,
                Technique = canva.Technique,
                DateOfEntry = canva.DateOfEntry,
                MuseumId = canva.MuseumId,
                Artists = canva.Works.Select(w => w.Artist.Name).ToList(),
                Comments = canva.Comments.Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CanvasId = c.CanvasId,
                    UserId = c.UserId,
                    Username = c.User.Username
                }).OrderByDescending(c => c.CreatedAt).ToList()
            };

            return Dto;
        }
        public async Task<Canvas?> Update(Guid id, UpdateCanvasDto dto)
        {
            var canvas = await _canvasRepo.GetById(id);
            if (canvas == null) throw new InvalidOperationException("Canvas ID not found");

            if (canvas.MuseumId != dto.MuseumId)
            {
                var newMuseum = await _museumRepo.GetById(dto.MuseumId);
                if (newMuseum == null) throw new InvalidOperationException("New Museum ID not found");
                canvas.MuseumId = dto.MuseumId;
            }

            canvas.Title = dto.Title;
            canvas.Technique = dto.Technique;
            canvas.DateOfEntry = dto.DateOfEntry;

            await _canvasRepo.Update(canvas);
            return canvas;
        }
    }
}
