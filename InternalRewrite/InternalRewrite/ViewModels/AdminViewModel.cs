using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InternalRewrite.Models;

namespace InternalRewrite.ViewModels
{
    public class AdminViewModel
    {
        public IEnumerable<User> UnlockableUsers { get; set; }
        public User NewAdminAccount { get; set; }
        public string creationMessage { get; set; }

        public AdminReport report { get; set; }
        //These are for reports
        
    }
}