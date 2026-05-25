
using BlogManagement.API.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BlogManagement.API.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Blogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Blogs)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Comments)
                    .WithOne(c => c.Blog)
                    .HasForeignKey(c => c.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ParentComment)
                    .WithMany(e => e.Replies)
                    .HasForeignKey(e => e.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            builder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Slug).HasMaxLength(50);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            builder.Entity<BlogTag>(entity =>
            {
                entity.HasKey(e => new { e.BlogId, e.TagId });
                entity.HasOne(e => e.Blog)
                    .WithMany(b => b.BlogTags)
                    .HasForeignKey(e => e.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tag)
                    .WithMany(t => t.BlogTags)
                    .HasForeignKey(e => e.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "C#", Slug = "csharp", Description = "C# programming language" },
                new Category { Id = 2, Name = "ASP.NET Core", Slug = "aspnet-core", Description = "ASP.NET Core framework" },
                new Category { Id = 3, Name = "SQL Server", Slug = "sql-server", Description = "Microsoft SQL Server" },
                new Category { Id = 4, Name = "JavaScript", Slug = "javascript", Description = "JavaScript programming language" },
                new Category { Id = 5, Name = "Design Patterns", Slug = "design-patterns", Description = "Software design patterns" }
            );
        }
    }
}
