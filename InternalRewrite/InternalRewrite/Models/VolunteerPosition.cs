namespace InternalRewrite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VolunteerPosition")]
    public partial class VolunteerPosition
    {
        public string Id { get; set; }

        
        [StringLength(128)]
        public string UserID { get; set; }

        [Required]
        [StringLength(128)]
        public string EventID { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        public string Description { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }
    }
}
