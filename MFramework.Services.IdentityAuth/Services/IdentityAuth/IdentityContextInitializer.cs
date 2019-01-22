using MFramework.Services.IdentityAuth.Constants;
using MFramework.Services.IdentityAuth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MFramework.Services.IdentityAuth
{
    public class IdentityContextInitializer : CreateDatabaseIfNotExists<IdentityContext>
    {
        protected override void Seed(IdentityContext context)
        {
            ApplyIdentityInitDataIsRequired(context);
        }

        public static void ApplyIdentityInitDataIsRequired(IdentityContext context)
        {
            Config.DefaultConfigService configService = new Config.DefaultConfigService();
            bool? applyIdentityDefaults = configService.GetAppSettingsValue<bool>("ApplyIdentityDefaultsIsEnabled");

            if (applyIdentityDefaults != null && applyIdentityDefaults == true)
            {
                // securityGroupAdmin
                SecurityGroupInitializeObject securityGroupAdmin =
                    new SecurityGroupInitializeObject
                    {
                        Name = "Admin",
                        RoleNames = new List<string> { RoleNames.UsersManager, RoleNames.RolesManager, RoleNames.SecurityGroupsManager }
                    };

                // securityGroupUser
                SecurityGroupInitializeObject securityGroupUser =
                    new SecurityGroupInitializeObject
                    {
                        Name = "User",
                        RoleNames = new List<string> { RoleNames.ExpenseReader, RoleNames.ExpenseCreator, RoleNames.ExpenseModifier }
                    };

                // securityGroups
                SecurityGroupInitializeObject[] securityGroups =
                    new SecurityGroupInitializeObject[] { securityGroupAdmin, securityGroupUser };



                // developers
                List<UserInitializeObject> developers = new List<UserInitializeObject>();

                // developer1
                UserInitializeObject developer1 = new UserInitializeObject
                {
                    Email = "muratbaseren@gmail.com",
                    SecurityGroups = new List<SecurityGroupInitializeObject> { securityGroupAdmin }
                };

                // developer2
                UserInitializeObject developer2 = new UserInitializeObject
                {
                    Email = "kadirmuratbaseren@gmail.com",
                    SecurityGroups = new List<SecurityGroupInitializeObject> { securityGroupUser }
                };

                developers.Add(developer1);
                developers.Add(developer2);

                AddInitialUsers(context, developers);
                AddInitialRoles(context);
                RemoveNonUsingRoles(context);
                AddInitialSecurityGroups(context, securityGroups);
                AddInitialUsersToSecurityGroup(context, developers);
                AddInitialUsersToRoles(context, developers);
            }
        }

        private static void AddInitialUsers(IdentityContext context, List<UserInitializeObject> users)
        {
            PasswordHasher passwordHasher = new PasswordHasher();

            foreach (var email in users.Select(x => x.Email))
            {
                if (context.Users.Any(x => x.Email == email)) continue;

                context.Users.Add(new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    UserName = email.Split('@')[0],
                    PasswordHash = passwordHasher.HashPassword("123456"),
                    EmailConfirmed = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                });
            }

            context.SaveChanges();
        }

        private static void AddInitialRoles(IdentityContext context)
        {
            Dictionary<string, string> roles = ConstantHelper.GetDisplayNamesDictionary<RoleNames>();

            foreach (var role in roles)
            {
                if (context.Roles.Any(x => x.Name == role.Key)) continue;

                context.Roles.Add(new ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = role.Key,
                    Title = role.Value,
                    Description = "Lorem ipsum dolor sit a met."
                });

                context.SaveChanges();
            }
        }

        private static void RemoveNonUsingRoles(IdentityContext context)
        {
            List<string> roleNames = typeof(RoleNames).GetFields().Select(x => x.GetValue(x).ToString()).ToList();

            foreach (var item in context.Roles.Where(x => roleNames.Contains(x.Name) == false))
            {
                context.Roles.Remove(context.Roles.Find(item.Id));
            }

            context.SaveChanges();
        }

        private static void AddInitialSecurityGroups(IdentityContext context, SecurityGroupInitializeObject[] securityGroups)
        {
            foreach (var item in securityGroups)
            {
                ApplicationSecurityGroup securityGroup = context.SecurityGroups.FirstOrDefault(x => x.Name == item.Name);
                bool isCreated = false;

                if (securityGroup == null)
                {
                    securityGroup = new ApplicationSecurityGroup()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = item.Name,
                        Description = "Lorem ipsum dolor sit a met."
                    };
                    isCreated = true;
                }
                securityGroup.Roles.Clear();
                context.SaveChanges();

                List<ApplicationRole> roles = context.Roles.Where(x => item.RoleNames.Contains(x.Name)).ToList();
                securityGroup.Roles.AddRange(roles);

                if (isCreated)
                {
                    context.SecurityGroups.Add(securityGroup);
                }

                context.SaveChanges();
            }
        }

        private static void AddInitialUsersToSecurityGroup(IdentityContext context, List<UserInitializeObject> developers)
        {
            foreach (var item in developers)
            {
                ApplicationUser user = context.Users.FirstOrDefault(x => x.Email == item.Email);
                user.SecurityGroups.Clear();
                context.SaveChanges();

                foreach (SecurityGroupInitializeObject secGroup in item.SecurityGroups)
                {
                    ApplicationSecurityGroup securityGroup = context.SecurityGroups.FirstOrDefault(x => x.Name == secGroup.Name);
                    user.SecurityGroups.Add(securityGroup);
                }

                context.SaveChanges();
            }
        }

        private static void AddInitialUsersToRoles(IdentityContext context, List<UserInitializeObject> developers)
        {
            foreach (var item in developers)
            {
                ApplicationUser user = context.Users.FirstOrDefault(x => x.Email == item.Email);
                user.Roles.Clear();
                context.SaveChanges();

                foreach (SecurityGroupInitializeObject secGroup in item.SecurityGroups)
                {
                    ApplicationSecurityGroup securityGroup = context.SecurityGroups.FirstOrDefault(x => x.Name == secGroup.Name);
                    securityGroup.Roles.ForEach(x => x.Users.Add(new IdentityUserRole { RoleId = x.Id, UserId = user.Id }));
                }

                context.SaveChanges();
            }
        }


        internal class SecurityGroupInitializeObject
        {
            public string Name { get; set; }
            public List<string> RoleNames { get; set; }

            public SecurityGroupInitializeObject()
            {
                RoleNames = new List<string>();
            }
        }

        internal class UserInitializeObject
        {
            public string Email { get; set; }
            public List<SecurityGroupInitializeObject> SecurityGroups { get; set; }

            public UserInitializeObject()
            {
                SecurityGroups = new List<SecurityGroupInitializeObject>();
            }

        }
    }
}