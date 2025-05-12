using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
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

        public ActionResult Index()
        {
            return View(db.Venues.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null) return HttpNotFound();

            return View(venue);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueId,VenueName,Location,Capacity")] Venue venue, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    venue.ImageUrl = GetBlobUrl(ImageFile);
                }

                db.Venues.Add(venue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(venue);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null) return HttpNotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueId,VenueName,Location,Capacity")] Venue venue, HttpPostedFileBase ImageFile)
        {
            var existingVenue = db.Venues.Find(venue.VenueId);
            if (existingVenue == null) return HttpNotFound();

            if (ModelState.IsValid)
            {
                existingVenue.VenueName = venue.VenueName;
                existingVenue.Location = venue.Location;
                existingVenue.Capacity = venue.Capacity;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    existingVenue.ImageUrl = GetBlobUrl(ImageFile);
                }

                db.Entry(existingVenue).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(venue);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venue = db.Venues.Find(id);
            if (venue == null) return HttpNotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var venue = db.Venues.Find(id);
            db.Venues.Remove(venue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
