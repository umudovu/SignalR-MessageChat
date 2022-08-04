using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalR_Intro.Data;
using SignalR_Intro.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Intro.SignalR
{
    public class MessageHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDataContext _context;

        public MessageHub(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
               var user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionId = Context.ConnectionId;

                _context.SaveChanges();
            }


            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionId = null;

                _context.SaveChanges();
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string userId, string message)
        {
            var user =await _userManager.FindByIdAsync(userId);


            var users = _context.Users.Where(x => x.ConnectionId != null).ToList();

            await Clients.Others.SendAsync("Users",users);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
