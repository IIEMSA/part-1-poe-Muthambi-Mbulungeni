using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

            // Handle missing configuration
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
            {
                throw new ApplicationException("Azure storage configuration is missing");
            }

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
                searchTerm = searchTerm.ToLower();
                venues = venues.Where(v =>
                    v.VenueName.ToLower().Contains(searchTerm) ||
                    v.Location.ToLower().Contains(searchTerm) ||
                    v.Capacity.ToString().Contains(searchTerm));
            }

            ViewBag.CurrentFilter = searchTerm;
            return View(venues.ToList());
        }

        // GET: Venue/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Venue venue = db.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // GET: Venue/Create
        public ActionResult Create()
        {
            return View(new Venue());
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueId,VenueName,Location,Capacity")] Venue venue, HttpPostedFileBase ImageFile)
        {
            try
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
                        else
                        {
                            venue.ImageUrl = null;
                        }

                        db.Venues.Add(venue);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Venue created successfully.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating venue: " + ex.Message);
            }

            return View(venue);
        }

        // GET: Venue/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null)
                return HttpNotFound();

            return View(venue);
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if another venue with same name and location exists (except current venue)
                    bool isDuplicate = db.Venues.Any(v =>
                        v.VenueId != venue.VenueId &&
                        v.VenueName == venue.VenueName &&
                        v.Location == venue.Location);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("", "Another venue with the same name and location already exists.");
                    }
                    else
                    {
                        // Handle image upload if a new image is provided
                        if (ImageFile != null && ImageFile.ContentLength > 0)
                        {
                            venue.ImageUrl = GetBlobUrl(ImageFile);
                        }

                        db.Entry(venue).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Venue updated successfully.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating venue: " + ex.Message);
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
            var venue = db.Venues
                .Include(v => v.Events)
                .Include(v => v.Bookings)
                .FirstOrDefault(v => v.VenueId == id);

            if (venue == null)
                return HttpNotFound();

            try
            {
                // Prevent deletion if events exist
                if (venue.Events.Any())
                {
                    TempData["DeleteError"] = "Cannot delete venue because it has scheduled events. " +
                                             "Delete the events first.";
                    return RedirectToAction("Delete", new { id });
                }

                // Prevent deletion if bookings exist
                if (venue.Bookings.Any())
                {
                    TempData["DeleteError"] = "Cannot delete venue because it has active bookings. " +
                                             "Delete the bookings first.";
                    return RedirectToAction("Delete", new { id });
                }

                // Safe to delete since no dependencies
                db.Venues.Remove(venue);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Venue deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["DeleteError"] = "Error deleting venue: " + ex.Message;
                return RedirectToAction("Delete", new { id });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
