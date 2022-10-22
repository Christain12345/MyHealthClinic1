using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyHealthClinic.Models;

namespace MyHealthClinic.Controllers
{
    public class ConsultsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Consults
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Consults.ToList());
            }
            return HttpNotFound();
        }

        // GET:MyConsults/id
        public ActionResult MyConsults()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Patient"))
            {
                var model = db.Consults
                    .Where(c => c.Patient.Id == userId)
                    .Include(c => c.AvailableTime)
                    .Include("AvailableTime.GeneralPractioner")
                    .ToList();
                return View(model);
            }
            else if (User.IsInRole("General Practitioner"))
            {
                var model = db.Consults
                   .Where(c => c.AvailableTime.GeneralPractioner.Id == userId)
                   .Include(c => c.AvailableTime)
                   .Include(c => c.Patient)
                   .ToList();
                return View(model);
            }
            return HttpNotFound();
        }

        // GET: Create/id
        public ActionResult Create(int? id)
        {
            if (!User.IsInRole("Patient")) return HttpNotFound();
            if (id == null) return HttpNotFound();
            var availableTime = db.AvailableTimes.Find(id);
            if (availableTime == null) return HttpNotFound();
            availableTime.IsBooked = true;
            var userId = User.Identity.GetUserId();
            var patient = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var consult = new Consult
            {
                AvailableTime = availableTime,
                Patient = patient
            };
            db.Consults.Add(consult);
            db.Entry(availableTime).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyConsults");
        }

        // GET: Consults/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consult consult = db.Consults
                .Where(c => c.Id == id)
                .Include(c => c.Patient)
                .Include(c => c.AvailableTime)
                .Include(c => c.AvailableTime.GeneralPractioner)
                .FirstOrDefault();
            if (consult == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            if ((User.IsInRole("Patient") && userId != consult.Patient.Id) || (User.IsInRole("General Practitioner") && userId != consult.AvailableTime.GeneralPractioner.Id))
            {
                return HttpNotFound();
            }
            return View(consult);
        }

        // POST: Consults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consult consult = db.Consults
                .Where(c => c.Id == id)
                .Include(c => c.AvailableTime)
                .FirstOrDefault();
            var availableTime = consult.AvailableTime;
            availableTime.IsBooked = false;
            db.Consults.Remove(consult);
            db.Entry(availableTime).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyConsults");
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
