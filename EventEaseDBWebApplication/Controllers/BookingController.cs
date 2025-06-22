using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using EventEaseDBWebApplication.Models;

namespace EventEaseDBWebApplication.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEaseDB db = new EventEaseDB();

        // GET: Booking
        public ActionResult Index(
            string searchTerm,
            int? eventTypeId,
            DateTime? startDate,
            DateTime? endDate,
            bool? availableOnly)
        {
            var bookings = db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Event.EventType)
                .Include(b => b.Venue);

            // Search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchTerm) ||
                    b.Event.EventName.ToLower().Contains(searchTerm) ||
                    b.Venue.VenueName.ToLower().Contains(searchTerm));
            }

            // Event type filter
            if (eventTypeId.HasValue && eventTypeId > 0)
            {
                bookings = bookings.Where(b => b.Event.EventTypeId == eventTypeId.Value);
            }

            // Date range filter
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate <= endDate.Value);
            }

            // Availability filter
            if (availableOnly.HasValue && availableOnly.Value)
            {
                bookings = bookings.Where(b => b.Venue.IsAvailable);
            }

            // Populate filter dropdown
            ViewBag.EventTypeId = new SelectList(db.EventTypes, "EventTypeId", "Name", eventTypeId);
            ViewBag.CurrentFilter = searchTerm;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.AvailableOnly = availableOnly;

            return View(bookings.ToList());
        }

        // ... rest of the controller remains unchanged ...
    }
}