using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMan.Core.Models;

namespace PMan.DataService
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {

        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Ignore(c => c.TwoFactorEnabled)
                .Ignore(c => c.PhoneNumber)
                .Ignore(c => c.PhoneNumberConfirmed);

            builder.Entity<ApplicationUser>()
                .Property(e => e.Email)
                .IsRequired(true)
                .HasDefaultValue(null);

            builder.Entity<ApplicationUser>()
                .HasIndex(b => b.Email)
                .IsUnique();

            builder.Entity<ApplicationUser>()
                .Property(e => e.FirstName)
                .HasMaxLength(250)
                .IsRequired(true)
                .HasDefaultValue(null);

            builder.Entity<ApplicationUser>()
                .Property(e => e.LastName)
                .HasMaxLength(250)
                .IsRequired(true)
                .HasDefaultValue(null);

            builder.Entity<ApplicationUser>()
                .HasMany(p => p.Projects)
                .WithMany(p => p.Users)
                .UsingEntity<ProjectUser>(
                    j => j
                        .HasOne(pt => pt.Project)
                        .WithMany(t => t.ProjectUsers)
                        .HasForeignKey(pt => pt.ProjectId),
                    j => j
                        .HasOne(pt => pt.User)
                        .WithMany(p => p.ProjectUsers)
                        .HasForeignKey(pt => pt.UserId),
                    j =>
                    {
                        j.Property(pt => pt.Role);
                        j.HasKey(t => new { t.ProjectId, t.UserId });
                    }
                );
        }

    }
}