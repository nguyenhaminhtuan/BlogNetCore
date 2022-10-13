using BlogNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogNetCore.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Article> Articles { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configuration
        modelBuilder.Entity<Tag>()
            .HasQueryFilter(t => !t.IsDeleted)
            .HasIndex(t => t.Slug).IsUnique()
            ;
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.Status != UserStatus.Disabled)
            .HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Article>()
            .HasQueryFilter(a => a.Status != ArticleStatus.Deleted)
            .HasIndex(a => a.Slug).IsUnique();
        
        // Seeds
        modelBuilder.Entity<Tag>()
            .HasData(new List<Tag>()
            {
                new() { Id = 1, Name = "C#", Slug = "csharp" },
                new() { Id = 2, Name = "Java", Slug = "java" },
                new() { Id = 3, Name = "Javascript", Slug = "javascript" },
                new() { Id = 4, Name = "Go", Slug = "golang" },
                new() { Id = 5, Name = "C/C++", Slug = "c-and-c-plus-plus" },
                new() { Id = 7, Name = "ASP.NET Core", Slug = "asp-net-core" },
                new() { Id = 8, Name = "Spring boot", Slug = "spring-boot" },
                new() { Id = 9, Name = "ReactJS", Slug = "reactjs" },
                new() { Id = 10, Name = "Angular", Slug = "angular" }
            });
        
        base.OnModelCreating(modelBuilder);
    }
}