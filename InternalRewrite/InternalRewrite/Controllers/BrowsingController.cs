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
    public class BrowsingController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: Browsing base case. Creates the viewModel and sends it to Create, which then populates the page on a get,
        // and refines the search on a post.
        //[ActionName("StartIndex")]
        public ActionResult Index()
        {
            var viewModel = new BrowsingViewModel();
            BrowsingModel browsingObject = new BrowsingModel();
            viewModel.BrowseObject = browsingObject;
            //setup the viewModels information to be empty to start off
            viewModel.BrowseObject.Address = null;
            viewModel.BrowseObject.City = null;
            viewModel.BrowseObject.EventName = null;
            viewModel.BrowseObject.Position = null;
            viewModel.BrowseObject.State = null;
            viewModel.BrowseObject.StartDateTime = DateTime.MinValue;
            //not setting the endDateTime because I never actually use it?


            browsingObject.Address = "";
            browsingObject.City = null;
            browsingObject.EventName = null;
            browsingObject.Position = null;
            browsingObject.State = null;
            browsingObject.StartDateTime = DateTime.MinValue;


            //return RedirectToAction("Create", new { query = viewModel });
            return RedirectToAction("Create", new { eventName = browsingObject.EventName,
                position = browsingObject.Position,
                address = browsingObject.Address,
                city = browsingObject.City,
                state = browsingObject.State,
                startDate = browsingObject.StartDateTime});
        }
        
            // GET: Browsing/Details/5
            //changing this from DETAILS to Signing up for a position
        public ActionResult Details(string positionID)
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (positionID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VolunteerPosition pos = db.VolunteerPosition.Find(positionID);
            


            if (pos == null)
            {
                return HttpNotFound();
            }

            //check to see if the user can sign up, if not then return view to index. (or is it create)?
            //start and end time of the event you are signing up for:
            var startTime = db.VolunteerPosition.Find(positionID).Event.StartDateTime;
            var endTime = db.VolunteerPosition.Find(positionID).Event.EndDateTime;
            var allUsersPositions = from position in db.VolunteerPosition where position.UserID == userID select position;
            bool possibleToSignUp = true;
            //loop through each of the events that the user is currently a part of
            foreach(var position in allUsersPositions)
            {
                int less = DateTime.Compare(position.Event.EndDateTime, endTime);
                int greater = DateTime.Compare(position.Event.StartDateTime, startTime);
                //if the positions start time is between the startTime and endTime OR the end time is between, then reject!!
                if (  (DateTime.Compare(position.Event.StartDateTime, startTime) >= 0 && DateTime.Compare(position.Event.StartDateTime, endTime) <= 0)
                    || (DateTime.Compare(position.Event.EndDateTime, startTime) >= 0 && DateTime.Compare(position.Event.EndDateTime, endTime) <= 0) )
                {
                    possibleToSignUp = false;
                }
            }



            if (possibleToSignUp)
            {
                //VolunteerPosition vp = db.VolunteerPosition.Find(positionID);
                User creator = db.User.Find(pos.Event.UserID);
                // Event @event = db.Event.Find(id);

                ////notify All users about the Change.
                // IList<VolunteerPosition> effectedVols = (from vol in db.VolunteerPosition where (vol.EventID == @event.Id && vol.UserID.Length > 6) select vol).ToList();
                string message = "Just signed up for: " + pos.Event.EventName;

                Notification not = new Notification();

                var uniqueId = Guid.NewGuid().ToString();
                not.Id = uniqueId;
                not.Message = message;
                not.RecieverID = userID;
                not.SenderID = pos.Event.UserID;

                db.Notification.Add(not);
                db.SaveChanges();


                // set the Positions userID to the other user
                // User u = db.User.Find(userID);
                //pos.UserID = u.UserName;

                pos.UserID = userID;
                db.SaveChanges();

                return View(pos);
            }
            else
            {
                return RedirectToAction("Index", "Browsing");
            }
        }

        // GET: Browsing/Create
        //public ActionResult Create(BrowsingViewModel query)
        public ActionResult Create(string eventName, string position, string address, string city, string state, DateTime startDate)
        {
            var viewModel = new BrowsingViewModel();
            BrowsingModel browsingObject = new BrowsingModel();
            viewModel.BrowseObject = browsingObject;

            //set the viewModels browsing object information to the parameters:
            viewModel.BrowseObject.Address = address;
            viewModel.BrowseObject.City = city;
            viewModel.BrowseObject.EventName = eventName;
            viewModel.BrowseObject.Position = position;
            viewModel.BrowseObject.State = state;
            viewModel.BrowseObject.StartDateTime = startDate;
            //viewModel.BrowseObject.EventName = "res";


            IQueryable<Event> eNam, pos, sdate, addr, cit, sta;
            eNam = pos = sdate = addr = cit = sta = null;
            //set up query values to all be false

           // var searchResults = from even in db.Event select even;
            RemoveOldEvents();
            var searchResults = from even in db.Event select even;


            if (!string.IsNullOrEmpty(viewModel.BrowseObject.EventName))
            {
                eNam = from even in db.Event where even.EventName.Equals(viewModel.BrowseObject.EventName) select even;
                searchResults = searchResults.Intersect(eNam);
            }
            if (!string.IsNullOrEmpty(viewModel.BrowseObject.Address))
            {
                addr = from even in db.Event where even.Address.Equals(viewModel.BrowseObject.Address) select even;
                searchResults = searchResults.Intersect(addr);
            }
            if (!string.IsNullOrEmpty(viewModel.BrowseObject.City))
            {
                cit = from even in db.Event where even.City.Equals(viewModel.BrowseObject.City) select even;
                searchResults = searchResults.Intersect(cit);
            }
            if (!string.IsNullOrEmpty(viewModel.BrowseObject.State))
            {
                sta = from even in db.Event where even.State.Equals(viewModel.BrowseObject.State) select even;
                searchResults = searchResults.Intersect(sta);
            }
            if (viewModel.BrowseObject.StartDateTime != DateTime.MinValue)
            {
                sdate = from even in db.Event where 
                        (even.StartDateTime.Year == viewModel.BrowseObject.StartDateTime.Year &&
                        even.StartDateTime.Month == viewModel.BrowseObject.StartDateTime.Month &&
                        even.StartDateTime.Day == viewModel.BrowseObject.StartDateTime.Day )
                        select even;
                //sdate = from even in db.Event where System.DateTime.Equals(even.StartDateTime.Date, viewModel.BrowseObject.StartDateTime.Date) select even;
                searchResults = searchResults.Intersect(sdate);
            }
                /*}
            }*/


            var userID = User.Identity.GetUserId();

            //var viewModel = new BrowsingViewModel();

            foreach (var eve in searchResults)
            {
                //this should work, if not, set the inner to a var, then concat
                var temp = from vol in eve.VolunteerPosition select vol;


                //putting my Position query down to this if instead. only add positions according to search filter 
                if (!string.IsNullOrEmpty(viewModel.BrowseObject.Position))
                {
                    var posQuery = from vol in eve.VolunteerPosition where vol.Position.Equals(viewModel.BrowseObject.Position) select vol;
                    temp = temp.Intersect(posQuery);
                }
                //another query to filter out positions where the userID is null or empty
                var filterOut = from vol in eve.VolunteerPosition where string.IsNullOrEmpty(vol.UserID) select vol;
                temp = temp.Intersect(filterOut);



                if (temp != null)
                {
                    
                    if (viewModel.Volunteers == null )
                        viewModel.Volunteers = temp;
                    else
                        viewModel.Volunteers = viewModel.Volunteers.Concat(temp);
                }
            }

            viewModel.Events = searchResults;
            //viewModel.Volunteers = vols;
            //viewModel.BrowseObject = query.BrowseObject;

            return View(viewModel);
        }
        

        // POST: Browsing/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "EventName,Position,StartDateTime,Address,City,State")] BrowsingModel @que)
        public ActionResult Create(BrowsingViewModel model)
        {
            var userID = User.Identity.GetUserId();
            var checkDate = model.BrowseObject.StartDateTime;
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                //return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid || model.BrowseObject.StartDateTime == DateTime.MinValue)
            {
                //db.Event.Add(@event);
                //db.SaveChanges();
                var viewModel = new BrowsingViewModel();
                //viewModel.BrowseObject = @que;
                //return RedirectToAction("Create", new { query = viewModel });
                /*return RedirectToAction("Create", new
                {
                    eventName = @que.EventName,
                    position = @que.Position,
                    address = @que.Address,
                    city = @que.City,
                    state = @que.State,
                    startDate = @que.StartDateTime
                });*/
                return RedirectToAction("Create", new
                {
                    eventName = model.BrowseObject.EventName,
                    position = model.BrowseObject.Position,
                    address = model.BrowseObject.Address,
                    city = model.BrowseObject.City,
                    state = model.BrowseObject.State,
                    startDate = model.BrowseObject.StartDateTime
                });
            }
            //var userID = User.Identity.GetUserId();
            //ViewBag.UserID = new SelectList(db.User, "Id", "FirstName", @event.UserID);
            //ViewBag.UserID = new SelectList(userID, "Id", "FirstName", @event.UserID);


            //return View(@que);
            return View(model);
        }

        // GET: Browsing/Edit/5
        // changing this from 
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

        // POST: Browsing/Edit/5
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

        // GET: Browsing/Delete/5
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

        // POST: Browsing/Delete/5
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

        private void RemoveOldEvents()
        {
            IList<Event> events = (from even in db.Event select even).ToList();
            foreach (var eve in events)
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTime dt = eve.EndDateTime;
                DateTime timeUtc = DateTime.UtcNow;
                timeUtc = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                //if (dt >= DateTime.UtcNow) 
                if (dt <= timeUtc)
                {
                    IList<VolunteerPosition> vols = (from vol in eve.VolunteerPosition select vol).ToList();
                    foreach (var v in vols.ToList())
                    {
                        db.VolunteerPosition.Remove(v);
                    }
                    db.Event.Remove(eve);
                    db.SaveChanges();
                }
                
            }
        }
    }
}
