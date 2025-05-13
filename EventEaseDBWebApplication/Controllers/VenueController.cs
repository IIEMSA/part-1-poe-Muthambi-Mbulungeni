using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventEaseDBWebApplication.Models;

namespace EventEaseDBWebApplication.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseDB db = new EventEaseDB();

        private string GetBlobUrl(HttpPostedFileBase imageFile)
        {
            var connectionString = System.Configuration.ConfigurationManager.AppSettings["AzureStorageConnectionString"];
            var containerName = System.Configuration.ConfigurationManager.AppSettings["AzureBlobContainerName"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            blobClient.Upload(imageFile.InputStream, new BlobHttpHeaders { ContentType = imageFile.ContentType });
            return blobClient.Uri.ToString();
        }

        // GET: Venue
        public ActionResult Index(string searchTerm)
        {
            var venues = db.Venues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                venues = venues.Where(v =>
                    v.VenueName.Contains(searchTerm) ||
                    v.Location.Contains(searchTerm));
            }

            ViewBag.CurrentFilter = searchTerm;
            return View(venues.ToList());
        }

        // GET: Venue/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null) return HttpNotFound();

            return View(venue);
        }

        // GET: Venue/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueId,VenueName,Location,Capacity")] Venue venue, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = db.Venues.Any(v =>
                    v.VenueName == venue.VenueName &&
                    v.Location == venue.Location);

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "A venue with the same name and location already exists.");
                }
                else
                {
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        venue.ImageUrl = GetBlobUrl(ImageFile);
                    }

                    db.Venues.Add(venue);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Venue created successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(venue);
        }

        // GET: Venue/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null) return HttpNotFound();

            return View(venue);
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueId,VenueName,Location,Capacity")] Venue venue, HttpPostedFileBase ImageFile)
        {
            var existingVenue = db.Venues.Find(venue.VenueId);
            if (existingVenue == null) return HttpNotFound();

            if (ModelState.IsValid)
            {
                bool isDuplicate = db.Venues.Any(v =>
                    v.VenueId != venue.VenueId &&
                    v.VenueName == venue.VenueName &&
                    v.Location == venue.Location);

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "A venue with the same name and location already exists.");
                }
                else
                {
                    existingVenue.VenueName = venue.VenueName;
                    existingVenue.Location = venue.Location;
                    existingVenue.Capacity = venue.Capacity;

                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        existingVenue.ImageUrl = GetBlobUrl(ImageFile);
                    }

                    db.Entry(existingVenue).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(venue);
        }

        // GET: Venue/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null) return HttpNotFound();

            return View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var venue = db.Venues.Find(id);

            try
            {
                db.Venues.Remove(venue);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Venue deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData["DeleteError"] = "Venue can't be deleted because it is linked to events or bookings.";
                return RedirectToAction("Delete", new { id = id });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
