using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EventEaseDBWebApplication.Models;

namespace EventEaseDBWebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseDB db = new EventEaseDB();

        // GET: Event
        public ActionResult Index(string searchTerm)
        {
            var events = db.Events.Include(e => e.Venue);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                events = events.Where(e =>
                    e.EventName.Contains(searchTerm) ||
                    e.EventDate.ToString().Contains(searchTerm));
            }

            ViewBag.CurrentFilter = searchTerm;
            return View(events.ToList());
        }

        // GET: Event/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var @event = db.Events.Include(e => e.Venue).FirstOrDefault(e => e.EventId == id);
            if (@event == null) return HttpNotFound();

            return View(@event);
        }

        // GET: Event/Create
        public ActionResult Create()
        {
            PopulateVenueSelectList();
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = db.Events.Any(e =>
                    e.EventName == @event.EventName &&
                    DbFunctions.TruncateTime(e.EventDate) == DbFunctions.TruncateTime(@event.EventDate));

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "An event with the same name and date already exists.");
                }
                else
                {
                    db.Events.Add(@event);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Event created successfully.";
                    return RedirectToAction("Index");
                }
            }

            PopulateVenueSelectList(@event.VenueId);
            return View(@event);
        }

        // GET: Event/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var @event = db.Events.Find(id);
            if (@event == null) return HttpNotFound();

            PopulateVenueSelectList(@event.VenueId);
            return View(@event);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = db.Events.Any(e =>
                    e.EventId != @event.EventId &&
                    e.EventName == @event.EventName &&
                    DbFunctions.TruncateTime(e.EventDate) == DbFunctions.TruncateTime(@event.EventDate));

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "An event with the same name and date already exists.");
                }
                else
                {
                    db.Entry(@event).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Event updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            PopulateVenueSelectList(@event.VenueId);
            return View(@event);
        }

        // GET: Event/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var @event = db.Events.Find(id);
            if (@event == null) return HttpNotFound();

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var @event = db.Events.Find(id);

            try
            {
                db.Events.Remove(@event);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData["DeleteError"] = "Event can't be deleted because it exists in booking.";
                return RedirectToAction("Delete", new { id = id });
            }
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
