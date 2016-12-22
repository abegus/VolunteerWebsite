using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace InternalRewrite.Models
{
    public class BrowsingModel
    {
        [StringLength(30)]
        public string EventName { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDateTime { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime EndDateTime { get; set; }

        [StringLength(30)]
        public string Address { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(30)]
        public string State { get; set; }

        [StringLength(50)]
        public string Position { get; set; }
        
    }
}