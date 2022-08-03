using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignalR_Intro.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static SignalR_Intro.Helpers.Helper;

namespace SignalR_Intro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateUserFake()
        {

            List<AppUser> _users = new Faker<AppUser>()
                        .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                        .RuleFor(x => x.LastName, f => f.Name.LastName())
                        .RuleFor(x => x.Email, f => f.Person.Email)
                        .Generate(100);


            foreach (var item in _users)
            {
                await _userManager.CreateAsync(item, "Pa$$word123");
                await _userManager.AddToRoleAsync(item, UserRoles.Member.ToString());
            }



            return RedirectToAction("Index", "Home");
        }

    }
}
