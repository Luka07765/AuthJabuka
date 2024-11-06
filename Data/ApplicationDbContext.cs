
using Jade.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jade.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Post> Posts { get; set; } // Add Posts DbSet
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique(); // Prevent a user from liking the same post multiple times

            // Ignore IsActive property to prevent mapping to database
            modelBuilder.Entity<RefreshToken>()
                .Ignore(r => r.IsActive);

            // Additional configuration for RefreshToken if needed
            modelBuilder.Entity<RefreshToken>()
                .Property(r => r.RevokedByIp)
                .HasMaxLength(45); // Optional: Set max length for IP address
        }
    }

}
