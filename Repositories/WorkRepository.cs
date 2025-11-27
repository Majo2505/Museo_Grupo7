using Microsoft.EntityFrameworkCore;
using Museo.Data;
using Museo.Models;

namespace Museo.Repositories
{
    public class WorkRepository : IWorkRepository
    {
        private readonly AppDbContext _context;

        public WorkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Work work)
        {
            await _context.Works.AddAsync(work);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid canvasId, Guid artistId)
        {
            var work = await _context.Works.FirstOrDefaultAsync(w => w.CanvasId == canvasId && w.ArtistId == artistId);
            if (work == null) throw new InvalidOperationException("Work relation not found");
            _context.Works.Remove(work);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Work>> GetAll()
        {
            return await _context.Works.AsNoTracking().Include(w => w.Canvas).Include(w => w.Artist).ToListAsync();
        }

        public async Task<Work?> GetByIds(Guid canvasId, Guid artistId)
        {
            return await _context.Works.AsNoTracking().Include(w => w.Canvas).Include(w => w.Artist)
                .FirstOrDefaultAsync(w => w.CanvasId == canvasId && w.ArtistId == artistId);
        }

        public async Task Update(Work work)
        {
            _context.Works.Update(work);
            await _context.SaveChangesAsync();
        }
    }
}
