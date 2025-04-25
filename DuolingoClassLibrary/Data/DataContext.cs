using Microsoft.EntityFrameworkCore;
using DuolingoClassLibrary.Entities;

namespace DuolingoClassLibrary.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }
        public DbSet<MyCourse> MyCourses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for LeaderboardEntry
            modelBuilder.Entity<LeaderboardEntry>()
                .Property(e => e.Accuracy)
                .HasPrecision(18, 2);

            modelBuilder.Entity<LeaderboardEntry>()
                .Property(e => e.ScoreValue)
                .HasPrecision(18, 2);

            // Configure decimal precision for User
            modelBuilder.Entity<User>()
                .Property(e => e.Accuracy)
                .HasPrecision(18, 2);
        }
    }
}
