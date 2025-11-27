using Microsoft.EntityFrameworkCore;
using Museo.Data;
using Museo.Models;

namespace Museo.Repositories
{
    public class MuseumRepository : IMuseumRepository
    {
        private readonly AppDbContext _context;

        public MuseumRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Museum museum)
        {
            await _context.Museums.AddAsync(museum);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var museum = await _context.Museums.FirstOrDefaultAsync(m => m.Id == id);
            if (museum == null) throw new InvalidOperationException("Museum ID not found");
            _context.Museums.Remove(museum);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Museum>> GetAll()
        {
            return await _context.Museums.AsNoTracking().Include(m => m.Canvas).ThenInclude(c => c.Works)
                .ThenInclude(w => w.Artist).ToListAsync();
        }

        public async Task<Museum?> GetById(Guid id)
        {
            return await _context.Museums.AsNoTracking().Include(m => m.Canvas).ThenInclude(c => c.Works).ThenInclude(w => w.Artist).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task Update(Museum museum)
        {
            _context.Museums.Update(museum);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByName(string name)
        {
            var exist = _context.Museums.AnyAsync(m => m.Name.ToUpper() == name.ToUpper());
            return exist;
        }
    }
}
