using Atelier.Api._Entities;
using Microsoft.EntityFrameworkCore;

namespace Atelier.Api._Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<PlayerData> PlayerData { get; set; }
        public DbSet<PlayerLastResult> PlayerLastResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Player → Country (N-1)
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Country)
                .WithMany(c => c.Players)
                .HasForeignKey(p => p.CountryId);

            // Player → PlayerData (1-1)
            modelBuilder.Entity<PlayerData>()
                .HasOne(pd => pd.Player)
                .WithOne(p => p.Data)
                .HasForeignKey<PlayerData>(pd => pd.PlayerId);

            // PlayerData → PlayerLastResult (1-N)
            modelBuilder.Entity<PlayerLastResult>()
                .HasOne(lr => lr.PlayerData)
                .WithMany(pd => pd.LastResults)
                .HasForeignKey(lr => lr.PlayerDataId);
        }
    }
}
