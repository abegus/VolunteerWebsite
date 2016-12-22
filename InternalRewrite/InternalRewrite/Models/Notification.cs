namespace InternalRewrite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notification")]
    public partial class Notification
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string SenderID { get; set; }

        [Required]
        [StringLength(128)]
        public string RecieverID { get; set; }

        [Required]
        [StringLength(128)]
        public string Message { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
