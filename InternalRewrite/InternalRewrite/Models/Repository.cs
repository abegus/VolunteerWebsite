using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InternalRewrite.Models;

namespace InternalRewrite.Models
{
    public class Repository
    {
        private VolunteerModel db = new VolunteerModel();
        public List<VolunteerPosition> GetVolunteers(String eventID)
        {
            List<VolunteerPosition> eventVolunteers = new List<VolunteerPosition>();

            //eventVolunteers = db.VolunteerPosition.ToList().Where();

            return eventVolunteers;
        }
    }
}