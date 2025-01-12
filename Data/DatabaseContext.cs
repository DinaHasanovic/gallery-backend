using Microsoft.EntityFrameworkCore;

namespace AppBackEnd.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Painter> Painters { get; set; }
        public DbSet<Journalist> Journalists { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<ArtworkGrade> Grades { get; set; }
        public DbSet<JournalistVisit> Visits { get; set; }
        public DbSet<JuryMember> JuryMembers { get; set; }
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<ExhibitionArtwork> ExhibitionsArtworks { get; set; }
        public DbSet<Theme> Themes { get; set; }


        public DatabaseContext(DbContextOptions options) : base(options)
        { 

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasIndex(c => c.PTT)
                .IsUnique();
            modelBuilder.Entity<JuryMember>().HasIndex(j => j.JMBG).IsUnique(); 
            modelBuilder.Entity<Painter>().HasIndex( p => p.JMBG ).IsUnique();
            modelBuilder.Entity<User>().HasIndex( u => u.Email).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
