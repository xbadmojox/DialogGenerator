using DialogGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace DialogGenerator.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Quest> Quests { get; set; }
        public DbSet<QuestType> QuestTypes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<NPC> NPCs { get; set; }
        public DbSet<Weather> Weathers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quest>()
                .HasOne(q => q.QuestType)
                .WithMany()
                .HasForeignKey(q => q.QuestTypeId);

            modelBuilder.Entity<Quest>()
                .HasMany(q => q.RequiredItems)
                .WithMany();

            base.OnModelCreating(modelBuilder);
        }
    }
}