using System.ComponentModel.DataAnnotations;

namespace MFramework.Services.IdentityAuth.Constants
{
    // Specific for each action. Use one or more in Authorize attribute top on the action or controller..
    public class RoleNames
    {
        [Display(Name = "Users Manager")]
        public const string UsersManager = "UsersManager";

        [Display(Name = "Roles Manager")]
        public const string RolesManager= "RolesManager";

        [Display(Name = "Security Groups Manager")]
        public const string SecurityGroupsManager = "SecurityGroupsManager";

        [Display(Name = "Expense Creator")]
        public const string ExpenseCreator = "ExpenseCreator";

        [Display(Name = "Expense Modifier")]
        public const string ExpenseModifier = "ExpenseModifier";

        [Display(Name = "Expense Remover")]
        public const string ExpenseRemover = "ExpenseRemover";

        [Display(Name = "Expense Reader")]
        public const string ExpenseReader = "ExpenseReader";

        [Display(Name = "Expense Exporter")]
        public const string ExpenseExporter = "ExpenseExporter";
    }
}
