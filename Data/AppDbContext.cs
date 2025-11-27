using Microsoft.EntityFrameworkCore;
using Museo.Models;

namespace Museo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artist => Set<Artist>();
        public DbSet<Canvas> Canvas => Set<Canvas>();
        public DbSet<Work> Works => Set<Work>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Museum> Museums => Set<Museum>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>(a => {
                a.HasKey(artist => artist.Id);
                a.Property(artist => artist.Name).IsRequired().HasMaxLength(200);
                a.Property(artist => artist.Description).IsRequired().HasMaxLength(500);
                a.Property(artist => artist.Specialty).IsRequired().HasMaxLength(100);
                a.Property(artist => artist.TypeOfWork).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Canvas>(c =>
            {
                c.HasKey(canvas => canvas.Id);
                c.Property(canvas => canvas.Title).IsRequired().HasMaxLength(200);
                c.Property(canvas => canvas.Technique).IsRequired().HasMaxLength(300);
                c.Property(canvas => canvas.DateOfEntry).IsRequired();
            });

            modelBuilder.Entity<Work>(w =>
            {
                w.HasKey(w => new { w.CanvasId, w.ArtistId });
                w.HasOne(w => w.Canvas).WithMany(l => l.Works).HasForeignKey(o => o.CanvasId).OnDelete(DeleteBehavior.Cascade);
                w.HasOne(w => w.Artist).WithMany(a => a.Works).HasForeignKey(o => o.ArtistId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<City>(c =>
            {
                c.HasKey(city => city.Id);
                c.Property(city => city.Name).IsRequired().HasMaxLength(200);
                c.Property(city => city.Country).IsRequired().HasMaxLength(200);
                c.HasOne(city => city.Museum).WithOne(museum => museum.City).HasForeignKey<Museum>(museum => museum.CityId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Museum>(m =>
            {
                m.HasKey(museum => museum.Id);
                m.Property(museum => museum.Name).IsRequired().HasMaxLength(200);
                m.Property(museum => museum.Description).IsRequired().HasMaxLength(500);
                m.Property(museum => museum.OpeningYear).IsRequired();
                m.HasMany(museum => museum.Canvas).WithOne(canvas => canvas.Museum).HasForeignKey(canvas => canvas.MuseumId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
