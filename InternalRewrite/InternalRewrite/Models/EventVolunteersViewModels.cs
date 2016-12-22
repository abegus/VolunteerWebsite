using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace InternalRewrite.Models
{
    public partial class EventVolunteersViewModels 
    {
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<VolunteerPosition> Volunteers { get; set; }
        
    }
}