using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InternalRewrite.Models;

namespace InternalRewrite.ViewModels
{
    public class MyVolunteeringViewModel
    {

        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<VolunteerPosition> Volunteers { get; set; }

    }

}