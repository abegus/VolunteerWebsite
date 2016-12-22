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
using InternalRewrite.ViewModels;
using System.Data.Entity.Migrations;

namespace InternalRewrite.Controllers
{
    public class EventsAndViewsController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: EventsAndViews   Base Case
        [ActionName("StartIndex")]
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewModel = new EventsAndViews();
            //viewModel.Events = db.Event.Include(e => e.VolunteerPosition);

            //my query
            var events = from eve in db.Event where eve.UserID == userID select eve;

            IQueryable<VolunteerPosition> vols = null;
            //doing this part hard coded because I do not get any parameters as input
            string eventID = null; 
            /*if(events != null)
            {
                eventID = events.ToArray()[0].Id;
                ViewBag.EventID = eventID;
            }*/
            

            //I hope this works?
            viewModel.Events = events;
            viewModel.Volunteers = vols;

            return View(viewModel);
        }
        // GET: EventsAndViews   Select case
        public ActionResult Index(string id)
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewModel = new EventsAndViews();
            //viewModel.Events = db.Event.Include(e => e.VolunteerPosition);

            //my query
            var events = from eve in db.Event where eve.UserID == userID select eve;

            IQueryable<VolunteerPosition> vols = null;
            //add all of the volunteers for all of thea users events...
            /*foreach (var eve in events)
            {
                //this should work, if not, set the inner to a var, then concat
                var temp = from vol in db.VolunteerPosition where vol.EventID == eve.Id select vol;
                if (temp != null)
                {
                    if (vols == null)
                        vols = temp;
                    else
                        vols = vols.Concat(temp);
                }
            }*/
            
            if (id != null)
            {
                var vol = from v in db.VolunteerPosition where v.EventID.Equals(id) select v;
                ViewBag.EventID = id;
                vols = vol;
            }


            //I hope this works?
            viewModel.Events = events;
            viewModel.Volunteers = vols;

            return View(viewModel);
        }




        // GET: EventsAndViews/Details/5
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

        // GET: EventsAndViews/Create
        public ActionResult Create()
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["userId"] = userID;
            var uniqueId = Guid.NewGuid().ToString();

            Event myevent = new Models.Event();
            myevent.Id = uniqueId;
            myevent.UserID = User.Identity.GetUserId();
            
            ViewBag.UserID = new SelectList(userID, "Id", "FirstName");
            //ViewBag.UserID = new SelectList(db.User, "Id", "FirstName");
            return View(myevent);
        }

        // POST: EventsAndViews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,UserID,StartDateTime,EndDateTime,Address,City,State,NumVolunteers")] Event @event)
        {
            if (ModelState.IsValid)
            {
               /* var currentDateTime = DateTime.Today;
                var startDate = @event.StartDateTime;
                var endDate = @event.EndDateTime;

                //second if check inside so that it checks the models first, THEN(here) checks if the dates and stuff are valid
                if (startDate < endDate && currentDateTime < startDate)
                {*/
                    db.Event.Add(@event);
                    db.SaveChanges();
                    //return RedirectToAction("Index");

                return RedirectToAction("Create", "VolunteerPositions", new { EventID = @event.Id });
                //}
            }
            var userID = User.Identity.GetUserId(); 
            //ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            ViewBag.UserID = new SelectList(userID, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // GET: EventsAndViews/Edit/5
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
           // ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // POST: EventsAndViews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,UserID,StartDateTime,EndDateTime,Address,City,State,NumVolunteers")] Event @event)
        {
            Event oldEvent = db.Event.Find(@event.Id);
            if (ModelState.IsValid)
            {
                //notify All users about the Change.
                IList<VolunteerPosition> effectedVols = (from vol in db.VolunteerPosition where (vol.EventID == @event.Id && vol.UserID.Length > 6 ) select vol).ToList();
                string message = "The event: " + oldEvent.EventName + ", has been changed and It affects you. Check out the change.";
               // NotifyUsers(effectedVols,message);
                var userID = User.Identity.GetUserId();
                var sendingUser = db.User.Find(userID);
                foreach (var vol in effectedVols)
                {
                    Notification not = new Notification();
                    var effectedUser = db.User.Find(vol.UserID);
                    // not.User1 = effectedUser;
                    // not.User = sendingUser;
                    var uniqueId = Guid.NewGuid().ToString();
                    not.Id = uniqueId;
                    not.Message = message;
                    not.RecieverID = vol.UserID;
                    not.SenderID = userID;

                    db.Notification.Add(not);
                    db.SaveChanges();
                }

                //db.Entry(@event).State = EntityState.Modified;
                db.Set<Event>().AddOrUpdate(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        private void NotifyUsers(IList<VolunteerPosition> effectedVols, string message)
        {
            var userID = User.Identity.GetUserId();
            var sendingUser = db.User.Find(userID);
            foreach (var vol in effectedVols)
            {
                Notification not = new Notification();
                var effectedUser = db.User.Find(vol.UserID);
               // not.User1 = effectedUser;
               // not.User = sendingUser;
                var uniqueId = Guid.NewGuid().ToString();
                not.Id = uniqueId;
                not.Message = message;
                not.RecieverID = vol.UserID;
                not.SenderID = userID;
                
                db.Notification.Add(not);
                db.SaveChanges();
            }
        }

        // GET: EventsAndViews/Delete/5
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

        // POST: EventsAndViews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Event @event = db.Event.Find(id);

            ////notify All users about the Change.
            IList<VolunteerPosition> effectedVols = (from vol in db.VolunteerPosition where (vol.EventID == @event.Id && vol.UserID.Length > 6) select vol).ToList();
            string message = "The event: " + @event.EventName + ", has been Deleted, and you were signed up for it";
            // NotifyUsers(effectedVols,message);
            var userID = User.Identity.GetUserId();
            var sendingUser = db.User.Find(userID);
            foreach (var vol in effectedVols)
            {
                Notification not = new Notification();
                var effectedUser = db.User.Find(vol.UserID);
                // not.User1 = effectedUser;
                // not.User = sendingUser;
                var uniqueId = Guid.NewGuid().ToString();
                not.Id = uniqueId;
                not.Message = message;
                not.RecieverID = vol.UserID;
                not.SenderID = userID;

                db.Notification.Add(not);
                db.SaveChanges();
            }


            var vols = from vol in @event.VolunteerPosition select vol;
            foreach(var v in vols.ToList())
            {
                db.VolunteerPosition.Remove(v);
            }
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
