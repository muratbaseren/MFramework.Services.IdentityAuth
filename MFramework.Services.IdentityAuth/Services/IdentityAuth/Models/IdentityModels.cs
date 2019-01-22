using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MFramework.Services.IdentityAuth.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public partial class ApplicationUser : IdentityUser
    {
        public virtual List<ApplicationSecurityGroup> SecurityGroups { get; set; }

        public ApplicationUser()
        {
            SecurityGroups = new List<ApplicationSecurityGroup>();
        }

        public ApplicationUser(string userName) : base(userName)
        {
            SecurityGroups = new List<ApplicationSecurityGroup>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public partial class ApplicationRole : IdentityRole
    {
        [StringLength(160)]
        public string Title { get; set; }

        [StringLength(160)]
        public string Description { get; set; }

        public virtual List<ApplicationSecurityGroup> SecurityGroups { get; set; }

        public ApplicationRole()
        {
            SecurityGroups = new List<ApplicationSecurityGroup>();
        }

        public ApplicationRole(string name) : base(name)
        {
            SecurityGroups = new List<ApplicationSecurityGroup>();
        }
    }

    [Table("AspNetSecurityGroups")]
    public partial class ApplicationSecurityGroup
    {
        [Key]
        [StringLength(128)]
        public string Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(160)]
        public string Description { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }
        public virtual List<ApplicationRole> Roles { get; set; }

        public ApplicationSecurityGroup()
        {
            Users = new List<ApplicationUser>();
            Roles = new List<ApplicationRole>();
        }
    }
}