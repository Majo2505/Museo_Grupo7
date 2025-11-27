using Microsoft.EntityFrameworkCore;
using Museo.Data;
using Museo.Models;


namespace Museo.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;

        public CityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) throw new InvalidOperationException("City Id not found");
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<City>> GetAll()
        {
            return await _context.Cities.AsNoTracking().Include(c => c.Museum).ToListAsync();
        }

        public async Task<City?> GetById(Guid id)
        {
            return await _context.Cities.AsNoTracking().Include(c => c.Museum).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Update(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByName(string name)
        {
            var exist = _context.Cities.AnyAsync(c => c.Name.ToUpper() == name.ToUpper());
            return exist;
        }
    }
}
