using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyHealthClinic.Helpers;
using MyHealthClinic.Models;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyHealthClinic.Controllers
{
    public class GeneralPractitionersController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context = new ApplicationDbContext();


        public GeneralPractitionersController() { }

        public GeneralPractitionersController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: GeneralPractitioners
        public ActionResult Index()
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            var gpRole = rm.FindByName("General Practitioner");
            var gpList = UserManager.Users
                .Where(u => u.Roles.Any(r => r.RoleId == gpRole.Id))
                .Select(u => new ProfileViewModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    About = u.About,
                }).ToList();
            return View(gpList);
        }

        // GET: GeneralPractitioners/Details/5
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var user = UserManager.FindById(id);
            if (user == null) return HttpNotFound();
            var gpDetails = IdentityHelpers.GetProfileByUser(user);
            return View(gpDetails);
        }

        //
        // GET: GeneralPractitioners/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var user = UserManager.FindById(id);
            if (user == null) return HttpNotFound();
            var gpDetails = IdentityHelpers.GetProfileByUser(user);
            return View(gpDetails);
        }

        //
        // POST: GeneralPractitioners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Dob = model.Dob;
                user.Gender = model.Gender;
                user.About = model.About;

                var result = UserManager.Update(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("upate GP error", string.Join(",", result.Errors));
                    return View(model);
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //
        // GET: GeneralPractitioners/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                return View();
            }
            return HttpNotFound();
        }

        //
        // POST: GeneralPractitioners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateGpViewModel model)
        {
            if (!User.IsInRole("Admin")) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                About = model.About
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("create GP error", string.Join(",", result.Errors));
                return View(model);
            }
            UserManager.AddToRole(user.Id, "General Practitioner");
            return RedirectToAction("Index");
        }

    }
}
