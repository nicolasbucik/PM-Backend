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
        }

    }
}