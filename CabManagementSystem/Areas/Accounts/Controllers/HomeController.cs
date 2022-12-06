using CabManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CabManagementSystem.Areas.Accounts.Controllers
{
    [Area("Accounts")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid details");
                return View(model);
            }

            var res = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (res.Succeeded)
            {
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "User", new { Area = "Admin" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Cab_Driver"))
                    {
                        return RedirectToAction("Index", "User", new { Area = "Admin" });
                    }
                    else
                    {
                        //return RedirectToAction("Index", "Home", new { Area = "" });
                        return Redirect("/accounts/home/userhome");
                    }

                }
                //return RedirectToAction("Index", "Home", new {Area=""});
                //return Redirect("/accounts/login");
            }
            ModelState.AddModelError("", "Invalid details");
            return View(model);
        }








        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = Guid.NewGuid().ToString().Replace("-", "")
            };
            var role = Convert.ToString(model.UserTypes);
            switch (model.UserTypes)
            {
                case UserType.Cab_Driver:
                    break;
                case UserType.Cab_User:
                    break;
                default:
                    break;
            }

            var res = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, role);

            if (res.Succeeded)
                return RedirectToAction("Index", "Home", new { Area = "" });

            ModelState.AddModelError("", "An Error Occoured");
            return View(model);
            //return Redirect("/");
            //return RedirectToPage("Index", "Home", new { Area = "" });
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public async Task<IActionResult> GenerateDriver()
        {
            await _roleManager.CreateAsync(new IdentityRole() { Name = "Cab_Driver" });

            var users = await _userManager.GetUsersInRoleAsync("Cab_Driver");
            if (users.Count == 0)
            {
                var appUser = new ApplicationUser()
                {
                    FirstName = "Cab_Driver",
                    LastName = "User",
                    Email = "driver@driver.com",
                    UserName = "driver"
                };
                var res = await _userManager.CreateAsync(appUser, "Pass123#");
                await _userManager.AddToRoleAsync(appUser, "Cab_Driver");
            }
            return Ok("Driver Generated");






        }
        [HttpGet]
        public async Task<IActionResult> UserHome()
        {
            var signeduser = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByEmailAsync(signeduser.Email);



            return View(new RegisterViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            });
        }
        public async Task<IActionResult> GenerateUser()
        {
            await _roleManager.CreateAsync(new IdentityRole() { Name = "Cab_User" });
            await _roleManager.CreateAsync(new IdentityRole() { Name = "Cab_User" });

            var users = await _userManager.GetUsersInRoleAsync("Cab_User");
            if (users.Count == 0)
            {
                var appUser = new ApplicationUser()
                {
                    FirstName = "Cab_User",
                    LastName = "Cab_User",
                    Email = "user@user.com",
                    UserName = "user"
                };
                var res = await _userManager.CreateAsync(appUser, "Pass123#");
                await _userManager.AddToRoleAsync(appUser, "Cab_User");
            }
            return Ok("User Generated");








        }
    }
}
