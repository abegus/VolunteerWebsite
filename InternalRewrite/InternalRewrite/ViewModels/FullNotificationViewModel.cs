using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InternalRewrite.Models;

namespace InternalRewrite.ViewModels
{
    public class FullNotificationViewModel
    {
        public IEnumerable<FullNotificaitonModel> RecievedNotifications { get; set; }
        public FullNotificaitonModel sendingNotification { get; set; }
    }
}