using Microsoft.EntityFrameworkCore;

namespace Backend_2.Models
{
    public class TestContext: DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Solutions> Solutions { get; set; }
        public TestContext(DbContextOptions<TestContext> options): base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<Topic>().HasKey(x => x.Id);
            modelBuilder.Entity<Topic>().HasIndex(x => x.ParentId);
            modelBuilder.Entity<Topic>()
               .HasOne<Topic>()
               .WithMany()
               .HasForeignKey(topic => topic.ParentId)
               .HasPrincipalKey(topic => topic.Id)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasKey(x => x.RoleId);
            modelBuilder.Entity<User>()
               .HasOne<Role>()
               .WithMany()
               .HasForeignKey(user => user.RoleId)
               .HasPrincipalKey(role => role.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tasks>().HasKey(x => x.Id);
            modelBuilder.Entity<Tasks>()
                .HasOne<Topic>()
                .WithMany()
                .HasForeignKey(task => task.TopicId)
                .HasPrincipalKey(topic => topic.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solutions>().HasKey(x => x.Id);
            modelBuilder.Entity<Solutions>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.AuthorId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Solutions>()
               .HasOne<Tasks>()
               .WithMany()
               .HasForeignKey(s => s.TaskId)
               .HasPrincipalKey(u => u.Id)
               .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
