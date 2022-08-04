using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignalR_Intro.Data;
using SignalR_Intro.Models;
using SignalR_Intro.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static SignalR_Intro.Helpers.Helper;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SignalR_Intro.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDataContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public IActionResult Register()
        {
            RegisterVM registerVM = new RegisterVM();
            return View(registerVM);
        }

        public IActionResult Login()
        {
            LoginVM login = new LoginVM();
            return View(login);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = new AppUser
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,

            };

            IdentityResult result = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, UserRoles.Member.ToString());
            return RedirectToAction("login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM, string ReturnUrl)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);


            if (appUser == null)
            {
                ModelState.AddModelError("", "Email veya password invalid");
                return View(loginVM);
            }



            SignInResult result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, true, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Email veya password invalid");
                return View(loginVM);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email veya password invalid");
                return View(loginVM);
            }

            var roles = await _userManager.GetRolesAsync(appUser);

            
            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
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


        public async Task CreateRole()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
                }
            };
        }

    }
}
