// jauna klase, kur glabajas datu bazes savienojums ar pasu programmu
// prieks md 4 atkal balstijos uz gemini darbu secibu, ka taisit kodu 

using Microsoft.EntityFrameworkCore;
using HabitTracker.Domain;

namespace HabitTracker.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitRecord> HabitRecords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Habit>() // ka dati glabasies un uzvedisies datubaze
                .HasMany(h => h.Record)
                .WithOne()
                .HasForeignKey(r => r.HabitID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HabitRecord>()    // viens ieradums viena diena pec nosacijumiem
                .HasIndex(r => new { r.HabitID, r.CompletionDate })
                .IsUnique();
        }
    }
}