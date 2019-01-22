using MFramework.Services.IdentityAuth.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MFramework.Services.IdentityAuth.Controllers
{
    [Authorize(Roles = RoleNames.ExpenseReader)]
    public class ExpenseController : Controller
    {
        // GET: Expense
        [Authorize(Roles = RoleNames.ExpenseReader)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Expense/Details/5
        [Authorize(Roles = RoleNames.ExpenseReader)]
        public ActionResult Details()
        {
            return View();
        }

        // GET: Expense/Create
        [Authorize(Roles = RoleNames.ExpenseCreator)]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Expense/Edit/5
        [Authorize(Roles = RoleNames.ExpenseModifier)]
        public ActionResult Edit()
        {
            return View();
        }

        // GET: Expense/Delete/5
        [Authorize(Roles = RoleNames.ExpenseRemover)]
        public ActionResult Delete()
        {
            return View();
        }
    }
}
