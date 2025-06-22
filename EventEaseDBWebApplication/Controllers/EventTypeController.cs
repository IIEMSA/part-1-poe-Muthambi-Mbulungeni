using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EventEaseDBWebApplication.Models;

namespace EventEaseDBWebApplication.Controllers
{
    public class EventTypeController : Controller
    {
        private readonly EventEaseDB db = new EventEaseDB();

        // GET: EventType
        public ActionResult Index()
        {
            return View(db.EventTypes.ToList());
        }

        // GET: EventType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventType eventType = db.EventTypes.Find(id);
            if (eventType == null)
            {
                return HttpNotFound();
            }
            return View(eventType);
        }

        // GET: EventType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] EventType eventType)
        {
            if (ModelState.IsValid)
            {
                db.EventTypes.Add(eventType);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event type created successfully.";
                return RedirectToAction("Index");
            }
            return View(eventType);
        }

        // GET: EventType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventType eventType = db.EventTypes.Find(id);
            if (eventType == null)
            {
                return HttpNotFound();
            }
            return View(eventType);
        }

        // POST: EventType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventTypeId,Name,Description")] EventType eventType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventType).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event type updated successfully.";
                return RedirectToAction("Index");
            }
            return View(eventType);
        }

        // GET: EventType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventType eventType = db.EventTypes.Find(id);
            if (eventType == null)
            {
                return HttpNotFound();
            }
            return View(eventType);
        }

        // POST: EventType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventType eventType = db.EventTypes.Find(id);
            if (eventType == null)
            {
                return HttpNotFound();
            }

            // Check if any events use this type
            if (db.Events.Any(e => e.EventTypeId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete event type because it's used by events.";
                return RedirectToAction("Delete", new { id });
            }

            db.EventTypes.Remove(eventType);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Event type deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}