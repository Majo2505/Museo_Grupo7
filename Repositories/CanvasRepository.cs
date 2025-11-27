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
            if (canvas == null) throw new InvalidOperationException("Id de canvas no encontrado");
            _context.Canvas.Remove(canvas);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Canvas>> GetAll()
        {
            return await _context.Canvas.AsNoTracking().ToListAsync();
        }

        public async Task<Canvas?> GetById(Guid id)
        {
            return await _context.Canvas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Update(Canvas canvas)
        {
            _context.Canvas.Update(canvas);
            await _context.SaveChangesAsync();
        }
    }
}
