using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using EventEaseDBWebApplication.Models;

namespace EventEaseDBWebApplication.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEaseDB db = new EventEaseDB();

        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Event).Include(b => b.Venue);
            return View(bookings.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            return View(booking);
        }

        public ActionResult Create()
        {
            PopulateEventAndVenueSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateEventAndVenueSelectLists(booking.EventId, booking.VenueId);
            return View(booking);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            PopulateEventAndVenueSelectLists(booking.EventId, booking.VenueId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateEventAndVenueSelectLists(booking.EventId, booking.VenueId);
            return View(booking);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateEventAndVenueSelectLists(int? eventId = null, int? venueId = null)
        {
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", eventId);
            ViewBag.VenueId = new SelectList(db.Venues, "VenueId", "VenueName", venueId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
