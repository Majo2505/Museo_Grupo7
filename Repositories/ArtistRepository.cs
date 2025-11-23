using Microsoft.EntityFrameworkCore;
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
            await _context.Artistas.AddAsync(artista);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var artista = await _context.Artistas.FirstOrDefaultAsync(a => a.Id == id);
            if (artista == null) throw new InvalidOperationException("Id de artista no encontrado");
            _context.Artistas.Remove(artista);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByName(string name)
        {
            var exist = _context.Artistas.AnyAsync(a => a.Name.ToLower() == name.ToLower());
            return exist;
        }

        public async Task<IEnumerable<Artist>> GetAll()
        {
            return await _context.Artistas.AsNoTracking().ToListAsync();
        }

        public async Task<Artist?> GetById(Guid id)
        {
            return await _context.Artistas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Update(Artist artista)
        {
            _context.Artistas.Update(artista);
            await _context.SaveChangesAsync();
        }
    }
}
