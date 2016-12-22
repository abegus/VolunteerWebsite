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

namespace InternalRewrite.Controllers
{
    public class MyVolunteeringController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: MyVolunteering
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var viewModel = new MyVolunteeringViewModel();

            //my query to select all positions
            var positions = from pos in db.VolunteerPosition where pos.UserID == userID select pos;

            //now select all events of the positions you are in
            foreach (var pos in positions)
            {
                var events = from eve in db.Event where eve.Id == pos.EventID select eve;
                //base case, set it equal
                if (viewModel.Events == null)
                {
                    viewModel.Events = events;
                }
                else
                {
                    viewModel.Events = viewModel.Events.Concat(events);
                }
            }
            
            //I hope this works?
           // viewModel.Events = events;
            viewModel.Volunteers = positions;

            return View(viewModel);
        }

        // GET: MyVolunteering/Details/5
        public ActionResult Details(string positionId, string eventId)
        {
            if (positionId == null || eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(eventId);
            VolunteerPosition position = db.VolunteerPosition.Find(positionId);
            if (@event == null || position == null)
            {
                return HttpNotFound();
            }

            List < Event >  list = new List<Event>();
            list.Add(@event);
            List<VolunteerPosition> vlist = new List<VolunteerPosition>();
            vlist.Add(position);


            var viewModel = new MyVolunteeringViewModel();
            //viewModel.Events = (IEnumerable<Event>)@event;
            //viewModel.Volunteers = (IEnumerable<VolunteerPosition>)position;
            viewModel.Events = list;
            viewModel.Volunteers = vlist;

            return View(viewModel);
        }

        // GET: MyVolunteering/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName");
            return View();
        }

        // POST: MyVolunteering/Create
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
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // GET: MyVolunteering/Edit/5
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
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // POST: MyVolunteering/Edit/5
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
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            return View(@event);
        }

        // GET: MyVolunteering/Delete/5
        public ActionResult Delete(string positionId)
        {
            if (positionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VolunteerPosition position = db.VolunteerPosition.Find(positionId);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // POST: MyVolunteering/Delete/5
        //Withdraw from position
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string positionId)
        {
            VolunteerPosition position = db.VolunteerPosition.Find(positionId);
            var userID = User.Identity.GetUserId();
            //db.Event.Remove(@event);


            string message = "User: ["+ db.User.Find(userID).Email +"] just withdrew from position: ["+position.Position+"] in event: ["+ position.Event.EventName+"]";

            Notification not = new Notification();

            var uniqueId = Guid.NewGuid().ToString();
            not.Id = uniqueId;
            not.Message = message;
            not.SenderID = userID;
            not.RecieverID = position.Event.UserID;

            db.Notification.Add(not);
            db.SaveChanges();


            position.UserID = null;
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
