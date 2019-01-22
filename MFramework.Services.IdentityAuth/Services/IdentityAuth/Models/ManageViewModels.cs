using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations;

namespace MFramework.Services.IdentityAuth.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class UserCreateViewModel
    {
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; }
        public List<ApplicationSecurityGroup> UserSecurityGroups { get; set; }
        public List<ApplicationSecurityGroup> SecurityGroups { get; set; }
        public List<ApplicationRole> UserRoles { get; set; }
    }

    public class UserEditViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }

    public class RoleEditViewModel
    {
        [Required]
        public string RoleId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(160)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(160)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class RoleDetailsViewModel
    {
        public ApplicationRole Role { get; set; }
        public List<ApplicationSecurityGroup> RoleSecurityGroups { get; set; }
        public List<ApplicationUser> RoleApplicationUsers { get; set; }
    }

    public class SecurityGroupCreateViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(160)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class SecurityGroupEditViewModel
    {
        [Required]
        public string SecurityGroupId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(160)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class SecurityGroupDetailsViewModel
    {
        public ApplicationSecurityGroup SecurityGroup { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<ApplicationRole> ExceptRoles { get; internal set; }
        public List<ApplicationUser> ExceptUsers { get; internal set; }
    }
}