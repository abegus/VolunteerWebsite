using InternalRewrite.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternalRewrite.Controllers
{
    public class EventVolunteersController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: EventVolunteers
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var viewModel = new EventVolunteersViewModels();
            //viewModel.Events = db.Event.Include(e => e.VolunteerPosition);

                //my query
            var events = from eve in db.Event where eve.UserID == userID select eve;

            IQueryable<VolunteerPosition> vols = null;
            //add all of the volunteers for all of thea users events...
            foreach(var eve in events)
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
            }

            //I hope this works?
            viewModel.Events = events;
            viewModel.Volunteers = vols;

            return View(viewModel);
        }

        // GET: EventVolunteers/Details/5
        public ActionResult Details(int id)
        {




            return View();
        }

        // GET: EventVolunteers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventVolunteers/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EventVolunteers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EventVolunteers/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EventVolunteers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EventVolunteers/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
