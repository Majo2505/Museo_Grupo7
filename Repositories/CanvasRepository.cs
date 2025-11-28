using Microsoft.EntityFrameworkCore;
using Museo.Data;
using Museo.Models;

namespace Museo.Repositories
{
    public class CanvasRepository : ICanvasRepository
    {
        private readonly AppDbContext _context;

        public CanvasRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Canvas canvas)
        {
            await _context.Canvas.AddAsync(canvas);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var canvas = await _context.Canvas.FirstOrDefaultAsync(a => a.Id == id);
            if (canvas == null) throw new InvalidOperationException("Canvas Id not found");
            _context.Canvas.Remove(canvas);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Canvas>> GetAll()
        {
            return await _context.Canvas.AsNoTracking().Include(c => c.Works).ThenInclude(w => w.Artist).ToListAsync();
        }

       
        public async Task<Canvas?> GetById(Guid id)
        {
            return await _context.Canvas
                .AsNoTracking()
                .Include(c => c.Works)
                    .ThenInclude(w => w.Artist)
                .Include(c => c.Comments)          
                    .ThenInclude(com => com.User)   
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Update(Canvas canvas)
        {
            _context.Canvas.Update(canvas);
            await _context.SaveChangesAsync();
        }
    }
}
