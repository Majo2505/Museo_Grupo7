using Museo.Models;
using Museo.Models.Dtos.Work;
using Museo.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Museo.Services
{
    public class WorkService : IWorkService
    {
        private readonly IWorkRepository _workRepo;
        private readonly ICanvasRepository _canvasRepo;
        private readonly IArtistRepository _artistRepo;

        public WorkService(IWorkRepository workRepo, ICanvasRepository canvasRepo, IArtistRepository artistRepo)
        {
            _workRepo = workRepo;
            _canvasRepo = canvasRepo;
            _artistRepo = artistRepo;
        }

        public async Task<Work> Create(CreateWorkDto dto)
        {
            var existingWork = await _workRepo.GetByIds(dto.CanvasId, dto.ArtistId);
            if (existingWork != null)
            {
                throw new InvalidOperationException("The relation between this Artist and Canvas already exists");
            }

            var canvas = await _canvasRepo.GetById(dto.CanvasId);
            if (canvas == null)
            {
                throw new InvalidOperationException("Canvas ID not found");
            }

            var artist = await _artistRepo.GetById(dto.ArtistId);
            if (artist == null)
            {
                throw new InvalidOperationException("Artist ID not found");
            }

            var work = new Work
            {
                CanvasId = dto.CanvasId,
                ArtistId = dto.ArtistId
            };
            await _workRepo.Add(work);
            return work;
        }

        public async Task<bool> Delete(Guid canvasId, Guid artistId)
        {
            var existingWork = await _workRepo.GetByIds(canvasId, artistId);
            if (existingWork == null)
            {
                throw new InvalidOperationException("Relation not found");
            }

            await _workRepo.Delete(canvasId, artistId);
            return true;
        }

        public async Task<IEnumerable<Work>> GetAll()
        {
            return await _workRepo.GetAll();
        }

        public async Task<Work?> GetByRelation(Guid canvasId, Guid artistId)
        {
            return await _workRepo.GetByIds(canvasId, artistId);
        }
    }
}
