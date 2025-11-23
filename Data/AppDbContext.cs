using Microsoft.EntityFrameworkCore;
using Museo.Models;

namespace Museo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artistas => Set<Artist>();
        public DbSet<Canvas> Lienzos => Set<Canvas>();
        public DbSet<Work> Obras => Set<Work>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>(a => { 
                a.HasKey(artist => artist.Id);
                a.Property(artist => artist.Name).IsRequired().HasMaxLength(200);
                a.Property(artist => artist.Description).IsRequired().HasMaxLength(500);
                a.Property(artist => artist.Specialty).IsRequired().HasMaxLength(100);
                a.Property(artist => artist.TypeOfWork).IsRequired().HasMaxLength(100);
                //a.HasMany(artist=>artist.Works).WithOne(work=>work.Artist).HasForeignKey(work=>work.ArtistId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Canvas>(c =>
            {
                c.HasKey(canvas => canvas.Id);
                c.Property(canvas => canvas.Title).IsRequired().HasMaxLength(200);
                c.Property(canvas => canvas.Technique).IsRequired().HasMaxLength(300);
                c.Property(canvas => canvas.DateOfEntry).IsRequired();
                //c.HasMany(canvas => canvas.Works).WithOne(work => work.Canvas).HasForeignKey(work => work.CanvasId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Work>(w =>
            {
                w.HasKey(w => new { w.CanvasId, w.ArtistId });
                w.HasOne(w=> w.Canvas).WithMany(l=> l.Works).HasForeignKey(o=> o.CanvasId).OnDelete(DeleteBehavior.Cascade);
                w.HasOne(w=> w.Artist).WithMany(a=> a.Works).HasForeignKey(o=> o.ArtistId).OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
