using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

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
        modelBuilder.Entity<Article>()
            .HasMany(a => a.Tags)
            .WithMany(t => t.Articles)
            .UsingEntity<Dictionary<string, object>>(
                "ArticleTags",
                j => j
                    .HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("TagId"),
                j => j
                    .HasOne<Article>()
                    .WithMany()
                    .HasForeignKey("ArticleId"));
        
        modelBuilder.Entity<Tag>()
            .HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.Status != UserStatus.Disabled);
        modelBuilder.Entity<Article>()
            .HasQueryFilter(a => a.Status != ArticleStatus.Deleted);

        base.OnModelCreating(modelBuilder);
    }
}