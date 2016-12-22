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
    public class NotificationsController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: Notifications
        public ActionResult Index()
        {
            var viewModel = new FullNotificationViewModel();
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //var notification = db.Notification.Include(n => n.User).Include(n => n.User1);
            var recievedNotifications = from notification in db.Notification where notification.RecieverID == userID select notification;
            //loop through and add each one of these to the viewModel, along with the username
            foreach(var notification in recievedNotifications)
            {
                FullNotificaitonModel addToModel = new FullNotificaitonModel();
                addToModel.Id = notification.Id;
                addToModel.Message = notification.Message;
                addToModel.RecieverID = notification.RecieverID;
                addToModel.SenderID = notification.SenderID;
                //should get username?
                addToModel.SenderUsername = db.User.Find(notification.SenderID).UserName;
                
                //base case, set it equal
                if (viewModel.RecievedNotifications == null)
                {
                    //workaround incompatable datastructure
                    List<FullNotificaitonModel> list = new List<FullNotificaitonModel>();
                    list.Add(addToModel);
                    viewModel.RecievedNotifications = list;
                }
                else
                {
                    //workaround incompatable datastructure
                    List<FullNotificaitonModel> list = new List<FullNotificaitonModel>();
                    list.Add(addToModel);
                    //viewModel.RecievedNotifications = list;

                    viewModel.RecievedNotifications = viewModel.RecievedNotifications.Concat(list);
                }
            }

            viewModel.sendingNotification = new FullNotificaitonModel();
            //give the potential new notification a unique id
            var uniqueId = Guid.NewGuid().ToString();
            viewModel.sendingNotification.Id = uniqueId;
            viewModel.sendingNotification.SenderID = userID;

            //viewModel.sendingNotification
            //set viewbag.userID to the current users ID to be used for sending a new notification
            ViewBag.userID = userID;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( FullNotificationViewModel viewModel)
        {

            var shouldBeSingleId = from us in db.User where (us.Email.Equals(viewModel.sendingNotification.RecieverUsername)) select us;
            //set these by default
            bool ifFound = false;
            ViewBag.error = "";

            foreach (var id in shouldBeSingleId)
            {
                ifFound = true;
                viewModel.sendingNotification.RecieverID = id.Id;
                break;
            }
            //check validity?
            //if (ModelState.IsValid && ifFound)
            if (ifFound && viewModel.sendingNotification.Message != null)
            {
                //transfer all information over to a Notification model
                Notification addToDb = new Notification();
                addToDb.Id = viewModel.sendingNotification.Id;
                addToDb.SenderID = viewModel.sendingNotification.SenderID;
                addToDb.RecieverID = viewModel.sendingNotification.RecieverID;
                addToDb.Message = viewModel.sendingNotification.Message;


                db.Notification.Add(addToDb);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (!ifFound)
            {
                ViewBag.error = "Not such user: [" +viewModel.sendingNotification.RecieverUsername +"] exists in the system. ";
            }
            if(viewModel.sendingNotification.Message == null)
            {
                //ViewBag.error += "Invalid Massage";
            }
            ViewBag.RecieverID = new SelectList(db.User, "Id", "FirstName", viewModel.sendingNotification.RecieverID);
            ViewBag.SenderID = new SelectList(db.User, "Id", "FirstName", viewModel.sendingNotification.SenderID);



            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //var notification = db.Notification.Include(n => n.User).Include(n => n.User1);
            var recievedNotifications = from notification in db.Notification where notification.RecieverID == userID select notification;
            //loop through and add each one of these to the viewModel, along with the username
            foreach (var notification in recievedNotifications)
            {
                FullNotificaitonModel addToModel = new FullNotificaitonModel();
                addToModel.Id = notification.Id;
                addToModel.Message = notification.Message;
                addToModel.RecieverID = notification.RecieverID;
                addToModel.SenderID = notification.SenderID;
                //should get username?
                addToModel.SenderUsername = db.User.Find(notification.SenderID).UserName;

                //base case, set it equal
                if (viewModel.RecievedNotifications == null)
                {
                    //workaround incompatable datastructure
                    List<FullNotificaitonModel> list = new List<FullNotificaitonModel>();
                    list.Add(addToModel);
                    viewModel.RecievedNotifications = list;
                }
                else
                {
                    //workaround incompatable datastructure
                    List<FullNotificaitonModel> list = new List<FullNotificaitonModel>();
                    list.Add(addToModel);
                    //viewModel.RecievedNotifications = list;

                    viewModel.RecievedNotifications = viewModel.RecievedNotifications.Concat(list);
                }
            }

            //return to index if the stuff is invalid?
            return View(viewModel);
        }



        // GET: Notifications/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            ViewBag.RecieverID = new SelectList(db.User, "Id", "FirstName");
            ViewBag.SenderID = new SelectList(db.User, "Id", "FirstName");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SenderID,RecieverUsername,Message")] FullNotificaitonModel notification)
        {
            var shouldBeSingleId = from us in db.User where (us.Email.Equals(notification.RecieverUsername)) select us;
            bool ifFound = false;
            foreach(var id in shouldBeSingleId)
            {
                ifFound = true;
                notification.RecieverID = id.Id;
                break;
            }
            //check validity?
            if (ModelState.IsValid && ifFound)
            {
                //transfer all information over to a Notification model
                Notification addToDb = new Notification();
                addToDb.Id = notification.Id;
                addToDb.SenderID = notification.SenderID;
                addToDb.RecieverID = notification.RecieverID;
                addToDb.Message = notification.Message;


                db.Notification.Add(addToDb);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RecieverID = new SelectList(db.User, "Id", "FirstName", notification.RecieverID);
            ViewBag.SenderID = new SelectList(db.User, "Id", "FirstName", notification.SenderID);

            //return to index if the stuff is invalid?
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.RecieverID = new SelectList(db.User, "Id", "FirstName", notification.RecieverID);
            ViewBag.SenderID = new SelectList(db.User, "Id", "FirstName", notification.SenderID);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SenderID,RecieverID,Message")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RecieverID = new SelectList(db.User, "Id", "FirstName", notification.RecieverID);
            ViewBag.SenderID = new SelectList(db.User, "Id", "FirstName", notification.SenderID);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);*/
            Notification notification = db.Notification.Find(id);
            db.Notification.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Notification notification = db.Notification.Find(id);
            db.Notification.Remove(notification);
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
