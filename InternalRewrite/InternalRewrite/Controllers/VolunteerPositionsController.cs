using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InternalRewrite.Models;
using Microsoft.AspNet.Identity;

namespace InternalRewrite.Controllers
{
    public class VolunteerPositionsController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: VolunteerPositions
        public ActionResult Index(string EventID)
        {
            var volunteerPositions = from pos in db.VolunteerPosition where pos.EventID.Equals(EventID) select pos;
            ViewBag.eventID = EventID;
            //var volunteer = from pos in db.VolunteerPosition select pos;
            //var va = db.VolunteerPosition.Include(v => v.Event).Include(v => v.User);
            //var volunteerPosition = db.VolunteerPosition.Include(v => v.Event).Include(v => v.User);
            return View(volunteerPositions.ToList());
        }

        // GET: VolunteerPositions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VolunteerPosition volunteerPosition = db.VolunteerPosition.Find(id);
            if (volunteerPosition == null)
            {
                return HttpNotFound();
            }
            var eventID = from pos in db.VolunteerPosition where pos.Id.Equals(id) select pos.EventID;
            ViewBag.eventID = eventID;
            return View(volunteerPosition);
        }

        // GET: VolunteerPositions/Create
        public ActionResult Create(string EventID)
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //ViewBag.EventID = new SelectList(db.Event, "Id", "EventName");
            //ViewBag.EventID = new SelectList(eventID, "Id", "EventName");
            var uniqueId = Guid.NewGuid().ToString();
            VolunteerPosition newPosition = new Models.VolunteerPosition();
            newPosition.EventID = EventID;
            newPosition.Id = uniqueId;
            ViewBag.randID = uniqueId;
            ViewBag.eventID = EventID;
            // ViewBag.UserID = new SelectList(db.User, "Id", "FirstName");
            return View(newPosition);
        }

        // POST: VolunteerPositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserID,EventID,Position,Description")] VolunteerPosition volunteerPosition)
        {
            if (ModelState.IsValid)
            {
                var eventID = volunteerPosition.EventID;
                db.VolunteerPosition.Add(volunteerPosition);
                db.SaveChanges();
                return RedirectToAction("Index","EventsAndViews", new { id=eventID});
            }
            

            ViewBag.EventID = new SelectList(db.Event, "Id", "EventName", volunteerPosition.EventID);
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", volunteerPosition.UserID);
            return View(volunteerPosition);
        }

        // GET: VolunteerPositions/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VolunteerPosition volunteerPosition = db.VolunteerPosition.Find(id);
            if (volunteerPosition == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Event, "Id", "EventName", volunteerPosition.EventID);
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", volunteerPosition.UserID);
            return View(volunteerPosition);
        }

        // POST: VolunteerPositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserID,EventID,Position,Description")] VolunteerPosition volunteerPosition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(volunteerPosition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Event, "Id", "EventName", volunteerPosition.EventID);
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", volunteerPosition.UserID);
            return View(volunteerPosition);
        }

        // GET: VolunteerPositions/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VolunteerPosition volunteerPosition = db.VolunteerPosition.Find(id);
            if (volunteerPosition == null)
            {
                return HttpNotFound();
            }
            return View(volunteerPosition);
        }

        // POST: VolunteerPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            VolunteerPosition volunteerPosition = db.VolunteerPosition.Find(id);
            db.VolunteerPosition.Remove(volunteerPosition);
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
