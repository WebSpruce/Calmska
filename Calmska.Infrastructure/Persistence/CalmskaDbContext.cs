using Calmska.Infrastructure.Persistence.Models;
using Calmska.Infrastructure.Persistence.Security;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Infrastructure.Persistence
{
    public class CalmskaDbContext : DbContext
    {
        public DbSet<AccountDocument> Accounts { get; set; }
        public DbSet<MoodDocument> Moods { get; set; }
        public DbSet<MoodHistoryDocument> MoodHistoryDb { get; set; }
        public DbSet<SettingsDocument> SettingsDb { get; set; }
        public DbSet<TipsDocument> TipsDb { get; set; }
        public DbSet<Types_TipsDocument> Types_TipsDb { get; set; }
        public DbSet<Types_MoodDocument> Types_MoodDb { get; set; }
        public CalmskaDbContext(DbContextOptions options) : base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccountDocument>();
            modelBuilder.Entity<MoodDocument>();
            modelBuilder.Entity<MoodHistoryDocument>();
            modelBuilder.Entity<SettingsDocument>();
            modelBuilder.Entity<TipsDocument>();
            modelBuilder.Entity<Types_TipsDocument>()
                .Property(t => t.TypeId)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<IntValueGenerator>();
            modelBuilder.Entity<Types_MoodDocument>()
                .Property(t => t.TypeId)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<IntValueGenerator>();
        }
    }
}
