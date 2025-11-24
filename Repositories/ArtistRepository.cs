using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Museo.Data;
using Museo.Models;

namespace Museo.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly AppDbContext _context;

        public ArtistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Artist artista)
        {
            await _context.Artist.AddAsync(artista);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var artista = await _context.Artist.FirstOrDefaultAsync(a => a.Id == id);
            if (artista == null) throw new InvalidOperationException("Id dont found");
            _context.Artist.Remove(artista);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByName(string name)
        {
            var exist = _context.Artist.AnyAsync(a => a.Name.ToLower() == name.ToLower());
            return exist;
        }

        public async Task<IEnumerable<Artist>> GetAll()
        {
            return await _context.Artist.AsNoTracking().ToListAsync();
        }

        public async Task<Artist?> GetById(Guid id)
        {
            return await _context.Artist.AsNoTracking().Include(a => a.Works).ThenInclude(w => w.Canvas).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Update(Artist artista)
        {
            _context.Artist.Update(artista);
            await _context.SaveChangesAsync();
        }
    }
}
