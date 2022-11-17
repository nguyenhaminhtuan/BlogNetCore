using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Owner)
            .WithMany(u => u.Votes)
            .HasForeignKey(v => v.OwnerId);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Owner)
            .WithMany(u => u.Comments)
            .HasForeignKey(v => v.OwnerId);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ReplyTo)
            .WithMany(u => u.Replies)
            .HasForeignKey(v => v.ReplyToId);
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
        modelBuilder.Entity<Comment>()
            .HasQueryFilter(c => c.Article.Status != ArticleStatus.Deleted);
        modelBuilder.Entity<Vote>()
            .HasQueryFilter(v => v.Owner.Status != UserStatus.Disabled);

        base.OnModelCreating(modelBuilder);
    }
}