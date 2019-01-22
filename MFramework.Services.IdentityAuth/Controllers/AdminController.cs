using MFramework.Services.IdentityAuth;
using MFramework.Services.IdentityAuth.Constants;
using MFramework.Services.IdentityAuth.Controllers;
using MFramework.Services.IdentityAuth.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MFramework_Services_IdentityAuth.Controllers
{
    public class AdminController : IdentityBaseController
    {
        public AdminController()
        {

        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager) : base(userManager, signInManager, roleManager)
        {

        }


        #region User-List

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult ManageUsers()
        {
            return View();
        }

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult GetUserTable()
        {
            return PartialView("_UserTablePartial", UserManager.Users.ToList());
        }

        #endregion

        #region User-Create

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult CreateUser()
        {
            return PartialView("_UserCreateModalPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.UsersManager)]
        public async Task<ActionResult> CreateUser(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                PasswordHasher passwordHasher = new PasswordHasher();
                string hashedPassword = passwordHasher.HashPassword(model.Password);

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.Username,
                    PasswordHash = hashedPassword
                };

                var validationResult = await UserManager.UserValidator.ValidateAsync(user);

                if (validationResult.Succeeded)
                {
                    await UserManager.CreateAsync(user);
                    return JavaScript("createuser_success();");
                }
                else
                {
                    AddErrors(validationResult);
                }
            }

            return PartialView("_UserCreateFormPartial", model);
        }

        #endregion

        #region User-Edit

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult EditUser(string id)
        {
            ApplicationUser applicationUser = UserManager.FindById(id);
            UserEditViewModel model = new UserEditViewModel
            {
                UserId = applicationUser.Id,
                Email = applicationUser.Email,
                UserName = applicationUser.UserName
            };

            return PartialView("_UserEditModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.UsersManager)]
        public async Task<ActionResult> EditUser(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(model.UserId);
                user.Email = model.Email;
                user.UserName = model.UserName;

                var validationResult = await UserManager.UserValidator.ValidateAsync(user);

                if (validationResult.Succeeded)
                {
                    await UserManager.UpdateAsync(user);
                    return JavaScript("edituser_success();");
                }
                else
                {
                    AddErrors(validationResult);
                }
            }

            return PartialView("_UserEditFormPartial", model);
        }

        #endregion

        #region User-Details

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult DetailsUser(string id)
        {
            ApplicationUser applicationUser = UserManager.FindById(id);
            UserDetailsViewModel model = new UserDetailsViewModel
            {
                User = applicationUser
            };
            return PartialView("_UserDetailsModalPartial", model);
        }

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult DetailsUserData(string id)
        {
            ApplicationUser applicationUser = UserManager.FindById(id);
            string[] userRolesIds = applicationUser.Roles.Select(x => x.RoleId).ToArray();
            List<ApplicationRole> userRoles = IdentityContext.Roles.Where(x => userRolesIds.Contains(x.Id)).ToList();

            UserDetailsViewModel model = new UserDetailsViewModel
            {
                User = applicationUser,
                SecurityGroups = IdentityContext.SecurityGroups.ToList(),
                UserSecurityGroups = applicationUser.SecurityGroups.ToList(),
                UserRoles = userRoles
            };

            return PartialView("_UserDetailsDataPartial", model);
        }

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult DetailsUserSecurityGroupRoles(string id)
        {
            List<ApplicationRole> roles = IdentityContext.SecurityGroups.Find(id)?.Roles.ToList();
            return base.PartialView("_UserSecurityGroupRolesPartial", roles);
        }

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult DetailsRemoveUserSecurityGroup(string userId, string securityGroupId)
        {
            ApplicationUser applicationUser = IdentityContext.Users.Find(userId);
            ApplicationSecurityGroup securityGroup = applicationUser.SecurityGroups.FirstOrDefault(x => x.Id == securityGroupId);

            securityGroup.Roles.ForEach(r =>
            {
                var roleSecGroupsIds = r.SecurityGroups.Select(x => x.Id).ToArray();
                var exceptSecGroups = r.SecurityGroups.Where(x => roleSecGroupsIds.Contains(x.Id) && x.Id != securityGroupId).ToList();

                if (exceptSecGroups != null && exceptSecGroups.Count > 0)
                {
                    //Eğer kullanıcının silinecek SG 'undan gelen rol'ü.Başka SG'lerde de varsa
                    // Silinen SG'un dışındaki SG'ları için..
                    if (exceptSecGroups.Any(sg => sg.Users.Any(x => x.Id == applicationUser.Id)))
                    {
                        //Silinmeye çalışılan SG'un rollerinden şu an işlenen rol için,
                        // Bu kullanıcı başka SG üzerinden bu rol'e aittir. 
                        // Yani kullanıcı rol'den kaldırılmamalı.
                        // Kaldırılırsa, diğer SG içinde olmasına rağmen rol kullanıcıdan silinmiş olur.
                    }
                    else
                    {
                        //Silinecek SG'un rolü için Kullanıcı başka SG 'lar üzerinden dahi bu rol'e dahil değil.
                        UserManager.RemoveFromRole(applicationUser.Id, r.Name);
                    }
                }
                else
                {
                    //Kullanıcının silinecek SG'undaki rol, başka SG'a ait değil.
                   UserManager.RemoveFromRole(applicationUser.Id, r.Name);
                }
            });

            UserManager.RemoveFromRoles(applicationUser.Id, securityGroup.Roles.Select(x => x.Name).ToArray());
            applicationUser.SecurityGroups.Remove(securityGroup);
            IdentityContext.SaveChanges();

            return DetailsUserData(userId);
        }

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult DetailsAddSecurityGroupToUser(string userId, string securityGroupId)
        {
            ApplicationUser applicationUser = UserManager.FindById(userId);
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.FirstOrDefault(x => x.Id == securityGroupId);
            List<string> roleNames = securityGroup.Roles.Select(x => x.Name).ToList();

            roleNames.ForEach(rname => UserManager.AddToRole(applicationUser.Id, rname));

            applicationUser.SecurityGroups.Add(securityGroup);
            UserManager.Update(applicationUser);

            return DetailsUserData(userId);
        }

        #endregion

        #region User-Delete

        [Authorize(Roles = RoleNames.UsersManager)]
        public ActionResult DeleteUser(string id)
        {
            ApplicationUser applicationUser = UserManager.FindById(id);

            return PartialView("_UserDeleteModalPartial", applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteUser")]
        [Authorize(Roles = RoleNames.UsersManager)]
        public async Task<ActionResult> DeleteConfirmUser(string id)
        {
            ApplicationUser applicationUser = UserManager.FindById(id);
            applicationUser.SecurityGroups.Clear();
            UserManager.Update(applicationUser);

            await UserManager.DeleteAsync(applicationUser);

            return JavaScript("deleteuser_success();");
        }

        #endregion



        #region Role-List

        [Authorize(Roles = RoleNames.RolesManager)]
        public ActionResult ManageRoles()
        {
            var roles = RoleManager.Roles.ToList();
            return View(roles);
        }

        [Authorize(Roles = RoleNames.RolesManager)]
        public ActionResult GetRoleTable()
        {
            var roles = RoleManager.Roles.ToList();
            return PartialView("_RoleTablePartial", roles);
        }

        #endregion

        #region Role-Edit

        [Authorize(Roles = RoleNames.RolesManager)]
        public ActionResult EditRole(string id)
        {
            ApplicationRole applicationRole = RoleManager.FindById(id);
            RoleEditViewModel model = new RoleEditViewModel
            {
                RoleId = applicationRole.Id,
                Name = applicationRole.Name,
                Title = applicationRole.Title,
                Description = applicationRole.Description
            };

            return PartialView("_RoleEditModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.RolesManager)]
        public async Task<ActionResult> EditRole(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole applicationRole = RoleManager.FindById(model.RoleId);
                applicationRole.Title = model.Title;
                applicationRole.Description = model.Description;

                await RoleManager.UpdateAsync(applicationRole);

                return JavaScript("editrole_success();");
            }

            return PartialView("_RoleEditFormPartial", model);
        }

        #endregion

        #region Role-Details

        [Authorize(Roles = RoleNames.RolesManager)]
        public ActionResult DetailsRole(string id)
        {
            ApplicationRole applicationRole = RoleManager.FindById(id);
            RoleDetailsViewModel model = new RoleDetailsViewModel
            {
                Role = applicationRole
            };
            return PartialView("_RoleDetailsModalPartial", model);
        }

        [Authorize(Roles = RoleNames.RolesManager)]
        public ActionResult DetailsRoleData(string id)
        {
            ApplicationRole applicationRole = RoleManager.FindById(id);
            List<string> roleUserIds = applicationRole.Users.Select(x => x.UserId).ToList();
            RoleDetailsViewModel model = new RoleDetailsViewModel
            {
                Role = applicationRole,
                RoleSecurityGroups = applicationRole.SecurityGroups.ToList(),
                RoleApplicationUsers = IdentityContext.Users.Where(x => roleUserIds.Contains(x.Id)).ToList()
            };

            return PartialView("_RoleDetailsDataPartial", model);
        }

        #endregion


        #region SecurityGroup-List

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult ManageSecurityGroups()
        {
            return View();
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult GetSecurityGroupTable()
        {
            var securityGroups = IdentityContext.SecurityGroups.ToList();
            return PartialView("_SecurityGroupTablePartial", securityGroups);
        }

        #endregion

        #region SecurityGroup-Create

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult CreateSecurityGroup()
        {
            return PartialView("_SecurityGroupCreateModalPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult CreateSecurityGroup(SecurityGroupCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationSecurityGroup securityGroup = new ApplicationSecurityGroup
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Description = model.Description
                };

                IdentityContext.SecurityGroups.Add(securityGroup);
                IdentityContext.SaveChanges();

                return JavaScript("createsecuritygroup_success();");
            }

            return PartialView("_SecurityGroupCreateFormPartial", model);
        }

        #endregion

        #region SecurityGroup-Edit

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult EditSecurityGroup(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(id);
            SecurityGroupEditViewModel model = new SecurityGroupEditViewModel
            {
                SecurityGroupId = securityGroup.Id,
                Name = securityGroup.Name,
                Description = securityGroup.Description
            };

            return PartialView("_SecurityGroupEditModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult EditSecurityGroup(SecurityGroupEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(model.SecurityGroupId);
                securityGroup.Name = model.Name;
                securityGroup.Description = model.Description;

                IdentityContext.SaveChanges();

                return JavaScript("editsecuritygroup_success();");
            }

            return PartialView("_SecurityGroupEditFormPartial", model);
        }

        #endregion

        #region SecurityGroup-Details

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsSecurityGroup(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(id);
            SecurityGroupDetailsViewModel model = new SecurityGroupDetailsViewModel
            {
                SecurityGroup = securityGroup
            };
            return PartialView("_SecurityGroupDetailsModalPartial", model);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsSecurityGroupData(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(id);
            List<ApplicationRole> securityGroupRoles = securityGroup.Roles.ToList();
            List<ApplicationUser> securityGroupUsers = securityGroup.Users.ToList();
            string[] securityGroupRolesIds = securityGroupRoles.Select(x => x.Id).ToArray();
            string[] securityGroupUsersIds = securityGroupUsers.Select(x => x.Id).ToArray();
            List<ApplicationRole> securityGroupExceptRoles = IdentityContext.Roles.Where(x => securityGroupRolesIds.Contains(x.Id) == false).ToList();
            List<ApplicationUser> securityGroupExceptUsers = IdentityContext.Users.Where(x => securityGroupUsersIds.Contains(x.Id) == false).ToList();
            SecurityGroupDetailsViewModel model = new SecurityGroupDetailsViewModel
            {
                SecurityGroup = securityGroup,
                Roles = securityGroupRoles,
                Users = securityGroupUsers,
                ExceptRoles = securityGroupExceptRoles,
                ExceptUsers = securityGroupExceptUsers
            };

            return PartialView("_SecurityGroupDetailsDataPartial", model);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsSecurityGroupRoles(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(id);
            List<ApplicationRole> roles = securityGroup?.Roles.ToList();
            string[] rolesIds = roles.Select(x => x.Id).ToArray();
            List<ApplicationRole> exceptRoles = IdentityContext.Roles.Where(x => rolesIds.Contains(x.Id) == false).ToList();

            SecurityGroupDetailsViewModel model = new SecurityGroupDetailsViewModel
            {
                SecurityGroup = securityGroup,
                Roles = roles,
                ExceptRoles = exceptRoles
            };
            return base.PartialView("_SecurityGroupRolesPartial", model);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsSecurityGroupUsers(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(id);
            List<ApplicationUser> users = securityGroup?.Users.ToList();
            string[] usersIds = users.Select(x => x.Id).ToArray();
            List<ApplicationUser> exceptUsers = IdentityContext.Users.Where(x => usersIds.Contains(x.Id) == false).ToList();

            SecurityGroupDetailsViewModel model = new SecurityGroupDetailsViewModel
            {
                SecurityGroup = securityGroup,
                Users = users,
                ExceptUsers = exceptUsers
            };

            return base.PartialView("_SecurityGroupUsersPartial", model);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsRemoveUserFromSecurityGroup(string userId, string securityGroupId)
        {
            ApplicationUser applicationUser = IdentityContext.Users.Find(userId);
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(securityGroupId);

            securityGroup.Users.Remove(applicationUser);
            IdentityContext.SaveChanges();

            securityGroup.Roles.ForEach(role =>
            {
                //Silinecek role başka bir sec grp da var mı? Varsa kullanıcıdan silmemeliyiz.
                bool hasAnotherSecGroupThatRoleForUser =
                    applicationUser.SecurityGroups.Where(x => x.Id != securityGroupId).ToList()
                        .Any(x => x.Roles.Any(r => r.Id == role.Id));

                if (hasAnotherSecGroupThatRoleForUser == false)
                {
                    UserManager.RemoveFromRole(applicationUser.Id, role.Name);
                }
            });

            return DetailsSecurityGroupUsers(securityGroupId);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsAddUserToSecurityGroup(string userId, string securityGroupId)
        {
            ApplicationUser applicationUser = IdentityContext.Users.Find(userId);
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(securityGroupId);

            securityGroup.Users.Add(applicationUser);
            IdentityContext.SaveChanges();

            securityGroup.Roles.ForEach(r =>
            {
                UserManager.AddToRole(applicationUser.Id, r.Name);
            });

            return DetailsSecurityGroupUsers(securityGroupId);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsRemoveRoleFromSecurityGroup(string roleId, string securityGroupId)
        {
            ApplicationRole applicationRole = IdentityContext.Roles.Find(roleId);
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(securityGroupId);

            securityGroup.Roles.Remove(applicationRole);
            IdentityContext.SaveChanges();

            securityGroup.Users.ForEach(u =>
            {
                //Silinecek role başka bir sec grp da var mı? Varsa kullanıcıdan silmemeliyiz.
                bool hasAnotherSecGroupThatRoleForUser =
                    u.SecurityGroups.Where(x => x.Id != securityGroupId).ToList()
                        .Any(x => x.Roles.Any(r => r.Id == roleId));

                if (hasAnotherSecGroupThatRoleForUser == false)
                {
                    UserManager.RemoveFromRole(u.Id, applicationRole.Name);
                }
            });

            return DetailsSecurityGroupRoles(securityGroupId);
        }

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DetailsAddRoleToSecurityGroup(string roleId, string securityGroupId)
        {
            ApplicationRole applicationRole = IdentityContext.Roles.Find(roleId);
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(securityGroupId);

            securityGroup.Roles.Add(applicationRole);
            IdentityContext.SaveChanges();

            securityGroup.Users.ForEach(u => UserManager.AddToRole(u.Id, applicationRole.Name));

            return DetailsSecurityGroupRoles(securityGroupId);
        }

        #endregion

        #region SecurityGroup-Delete

        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DeleteSecurityGroup(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups.Find(id);

            return PartialView("_SecurityGroupDeleteModalPartial", securityGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteSecurityGroup")]
        [Authorize(Roles = RoleNames.SecurityGroupsManager)]
        public ActionResult DeleteConfirmSecurityGroup(string id)
        {
            ApplicationSecurityGroup securityGroup = IdentityContext.SecurityGroups
                .Include(x => x.Roles).Include(x => x.Users)
                .FirstOrDefault(x => x.Id == id);

            securityGroup.Users.ForEach(u =>
            {
                securityGroup.Roles.ForEach(role =>
                {
                    // Silinecek role başka bir sec grp da var mı? Varsa kullanıcıdan silmemeliyiz.
                    bool hasAnotherSecGroupThatRoleForUser =
                        u.SecurityGroups.Where(x => x.Id != id).ToList()
                            .Any(x => x.Roles.Any(r => r.Id == role.Id));

                    if (hasAnotherSecGroupThatRoleForUser == false)
                    {
                        UserManager.RemoveFromRole(u.Id, role.Name);
                    }
                });
            });

            IdentityContext.SecurityGroups.Remove(securityGroup);
            IdentityContext.SaveChanges();

            return JavaScript("deletesecuritygroup_success();");
        }

        #endregion
    }
}
