using Lingo.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lingo.Infrastructure.Storage
{
    //DO NOT TOUCH THIS FILE!!
    internal class LingoDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public LingoDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("ExternalLogins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        }
    }
}
