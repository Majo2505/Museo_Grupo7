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

        public async Task Add(Canvas lienzo)
        {
            await _context.Lienzos.AddAsync(lienzo);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var lienzo = await _context.Lienzos.FirstOrDefaultAsync(a => a.Id == id);
            if (lienzo == null) throw new InvalidOperationException("Id de lienzo no encontrado");
            _context.Lienzos.Remove(lienzo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Canvas>> GetAll()
        {
            return await _context.Lienzos.AsNoTracking().ToListAsync();
        }

        public async Task<Canvas?> GetById(Guid id)
        {
            return await _context.Lienzos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Update(Canvas lienzo)
        {
            _context.Lienzos.Update(lienzo);
            await _context.SaveChangesAsync();
        }
    }
}
