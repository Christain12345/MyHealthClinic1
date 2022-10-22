using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using MyHealthClinic.Models;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;

namespace MyHealthClinic.Controllers
{
    public class AvailableTimesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: AvailableTimes/id?
        public ActionResult Index(string id)
        {
            var model = db.AvailableTimes
                .Where(at => DateTime.Compare(at.StartTime, DateTime.Now) > 0)
                .Include(at => at.GeneralPractioner)
                .ToList();
            if (id != null)
            {
                model = db.AvailableTimes
                    .Where(at => at.GeneralPractioner.Id == id)
                    .Where(at => DateTime.Compare(at.StartTime, DateTime.Now) > 0)
                    .Include(at => at.GeneralPractioner)
                    .ToList();
            }
            return View(model);
        }

        // GET: AvailableTimes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailableTime availableTime = db.AvailableTimes.Find(id);
            if (availableTime == null)
            {
                return HttpNotFound();
            }
            return View(availableTime);
        }

        // GET: AvailableTimes/Create
        public ActionResult Create()
        {
            if (User.IsInRole("General Practitioner") || User.IsInRole("Admin"))
            {
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
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
                ViewBag.SelectedGeneralPractitioner = new SelectList(gpList, "Id", "FirstName");
                return View();
            }
            return HttpNotFound();
        }

        // POST: AvailableTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GeneralPractioner,StartTime,EndTime")] AvailableTime availableTime, string SelectedGeneralPractitioner)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("General Practitioner")) return HttpNotFound();

            if (User.IsInRole("General Practitioner"))
            {
                availableTime.GeneralPractioner = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            }
            else if (User.IsInRole("Admin"))
            {
                availableTime.GeneralPractioner = db.Users.Where(u => u.Id == SelectedGeneralPractitioner).FirstOrDefault();
            }
            if (DateTime.Compare(availableTime.StartTime, availableTime.EndTime) >= 0)
            {
                ModelState.AddModelError("Time", "End time must larger than start time");
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
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
                ViewBag.SelectedGeneralPractitioner = new SelectList(gpList, "Id", "FirstName");
                return View(availableTime);
            }
            db.AvailableTimes.Add(availableTime);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: AvailableTimes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("Patient")) return HttpNotFound();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailableTime availableTime = db.AvailableTimes.Include(at => at.GeneralPractioner).Where(at => at.GeneralPractioner.Id == id.ToString()).FirstOrDefault();
            if (User.IsInRole("General Practitioner"))
            {
                if (availableTime == null || User.Identity.GetUserId() != availableTime.GeneralPractioner.Id)
                {
                    return HttpNotFound();
                }
            }
            if (availableTime == null)
            {
                return HttpNotFound();
            }
            return View(availableTime);
        }

        // POST: AvailableTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartTime,EndTime")] AvailableTime availableTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(availableTime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(availableTime);
        }

        // GET: AvailableTimes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (User.IsInRole("Patient")) return HttpNotFound();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailableTime availableTime = db.AvailableTimes.Include(at => at.GeneralPractioner).Where(at => at.GeneralPractioner.Id == id.ToString()).FirstOrDefault();
            if (User.IsInRole("General Practitioner"))
            {
                if (availableTime == null || User.Identity.GetUserId() != availableTime.GeneralPractioner.Id)
                {
                    return HttpNotFound();
                }
            }
            if (availableTime == null)
            {
                return HttpNotFound();
            }
            return View(availableTime);
        }

        // POST: AvailableTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AvailableTime availableTime = db.AvailableTimes.Find(id);
            db.AvailableTimes.Remove(availableTime);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
