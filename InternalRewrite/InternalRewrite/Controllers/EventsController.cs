using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InternalRewrite.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace InternalRewrite.Controllers
{
    public class EventsController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: Events
        public ActionResult Index()
        {
            /* here is where I am going to have to return the view corresponding to the users events. 
             * So grab all events WHERE the userID on them is equal to: User.Identity.GetUserId(); or
             * HttpContext.Current.User.Identity.GetUserId(); , then return the view?*/
            var userID = User.Identity.GetUserId();
            //var x = User.Identity.
            //Event ev = db.Event
            //var temp;
            foreach (User u in db.User)
            {
                //temp.add(u);
            }
            //var retList = db.Event.ToList().Where<>
            //var eve = db.Event.Select<EventName>;
            //var eve = db.Event.Include(@ => @.User);

            var eve = db.Event.Include(u => u.User);
            //var event = db.Event.Include(@ => @.User);

            



            return View(eve.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {

            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if(userID == null)
            {
                return RedirectToAction("Login", "Account");
            }


            ViewData["userId"] = userID;
            var uniqueId = Guid.NewGuid().ToString();

            Event myevent = new Models.Event();
            myevent.Id = uniqueId;
            myevent.UserID = User.Identity.GetUserId();


            var vols = from even in db.Event where even.Id.Equals("caab29bb-52d5-4d98-92c0-a11bdf7899c7") select even;


            //ViewBag.UserID = new SelectList(userID, "Id", "FirstName");
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName");
            return View(myevent);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,UserID,StartDateTime,EndDateTime,Address,City,State,NumVolunteers")] Event @event)
        {
            
            if (ModelState.IsValid)
            {
                db.Event.Add(@event);
                db.SaveChanges();

                return RedirectToAction("Create", "VolunteerPositions", new { eventID = @event.Id });
 //overriding this to add volunteers
                return RedirectToAction("Index");
            }
    
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
//            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,UserID,StartDateTime,EndDateTime,Address,City,State,NumVolunteers")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "VolunteerPositions", new { eventID = @event.Id });
///override original index to send it to VOLUNTEERPOSITION INDEX
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Event @event = db.Event.Find(id);
            db.Event.Remove(@event);
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
