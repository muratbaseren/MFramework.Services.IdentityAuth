using MFramework.Services.IdentityAuth.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace MFramework.Services.IdentityAuth
{
    public partial class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public IDbSet<ApplicationSecurityGroup> SecurityGroups { get; set; }

        public IdentityContext()
#if DEBUG
            : base("DevelopmentConnection")
#else
            : base("ProductionConnection")
#endif
        {
            Database.SetInitializer(new IdentityContextInitializer());
        }

        public static IdentityContext Create()
        {
            return new IdentityContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationSecurityGroup>()
               .HasMany<ApplicationUser>(s => s.Users)
               .WithMany(c => c.SecurityGroups)
               .Map(cs =>
               {
                   cs.MapLeftKey("SecurityGroupId");
                   cs.MapRightKey("UserId");
                   cs.ToTable("AspNetSecurityGroupUsers");
               });

            modelBuilder.Entity<ApplicationSecurityGroup>()
               .HasMany<ApplicationRole>(s => s.Roles)
               .WithMany(c => c.SecurityGroups)
               .Map(cs =>
               {
                   cs.MapLeftKey("SecurityGroupId");
                   cs.MapRightKey("RoleId");
                   cs.ToTable("AspNetSecurityGroupRoles");
               });

            base.OnModelCreating(modelBuilder);
        }
    }
}