using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignalR_Intro.Data;
using SignalR_Intro.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Intro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDataContext _context;

        public HomeController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> ChatAsync()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("login","account");

            var users = _context.Users.Where(x=>x.ConnectionId!=null).ToList();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user.FirstName + " " + user.LastName;

            return View(users);
        }




       
       

    }
}
