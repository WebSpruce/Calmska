using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api
{
    public class CalmskaDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<MoodHistory> MoodHistoryDb { get; set; }
        public DbSet<Settings> SettingsDb { get; set; }
        public CalmskaDbContext(DbContextOptions options) : base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>();
            modelBuilder.Entity<Mood>();
            modelBuilder.Entity<MoodHistory>();
            modelBuilder.Entity<Settings>();
        }
    }
}
