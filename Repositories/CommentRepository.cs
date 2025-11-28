using Museo.Data;
using Museo.Models;
using Microsoft.EntityFrameworkCore;

namespace Museo.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetOne(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetByCanvasId(Guid canvasId)
        {
            return await _context.Comments
                .Include(c => c.User) 
                .Where(c => c.CanvasId == canvasId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        
        public async Task Add(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

     
        public async Task Update(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        
        public async Task Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
