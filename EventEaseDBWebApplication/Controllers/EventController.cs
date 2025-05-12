using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using EventEaseDBWebApplication.Models;

namespace EventEaseDBWebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseDB db = new EventEaseDB();

        public ActionResult Index()
        {
            var events = db.Events.Include(e => e.Venue);
            return View(events.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var @event = db.Events.Find(id);
            if (@event == null) return HttpNotFound();

            return View(@event);
        }

        public ActionResult Create()
        {
            PopulateVenueSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateVenueSelectList(@event.VenueId);
            return View(@event);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var @event = db.Events.Find(id);
            if (@event == null) return HttpNotFound();

            PopulateVenueSelectList(@event.VenueId);
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateVenueSelectList(@event.VenueId);
            return View(@event);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var @event = db.Events.Find(id);
            if (@event == null) return HttpNotFound();

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateVenueSelectList(int? selectedVenueId = null)
        {
            ViewBag.VenueId = new SelectList(db.Venues, "VenueId", "VenueName", selectedVenueId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
