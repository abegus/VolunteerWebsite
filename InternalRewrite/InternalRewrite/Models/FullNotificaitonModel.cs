using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace InternalRewrite.Models
{
    public class FullNotificaitonModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string SenderID { get; set; }

        [Required]
        [StringLength(128)]
        public string RecieverID { get; set; }

        public string RecieverUsername { get; set; }

        public string SenderUsername { get; set; }

        [Required]
        [StringLength(50)]
        public string Message { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}