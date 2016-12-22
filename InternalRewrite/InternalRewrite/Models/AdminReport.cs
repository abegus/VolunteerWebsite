using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace InternalRewrite.Models
{
    public class AdminReport
    {
        public int NumEvents { get; set; }
        public int NumVolunteerRoles { get; set; }
        public int NumNotifications { get; set; }
        public int NumFilledRoles { get; set; }
        
    }
}