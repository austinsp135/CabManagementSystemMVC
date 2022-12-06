using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace CabManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        
        private readonly ApplicationDbContext _db;



        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }



        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }



        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}



        //[HttpPost]
        //public async Task<IActionResult> Create(ApplicationUser model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);



        //    _db.Users.Add(new user()
        //    {
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
                
        //    });
        //    await _db.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _db.ApplicationUsers.FindAsync(id);
            if (user == null)
                return NotFound();



            return View(new ApplicationUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
                
            });
        }



        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            var user = await _db.ApplicationUsers.FindAsync(id);
            if (user == null)
                return NotFound();



            if (!ModelState.IsValid)
                return View(model);



            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();



            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        
    }
}

