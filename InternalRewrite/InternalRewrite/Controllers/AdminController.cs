using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InternalRewrite.Models;
using Microsoft.AspNet.Identity;
using InternalRewrite.ViewModels;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;

namespace InternalRewrite.Controllers
{
    public class AdminController : Controller
    {
        private VolunteerModel db = new VolunteerModel();

        // GET: Admin
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            //checking to see if anyone is logged in, if not, redirect to login
            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //check to see if the user is an admin user, if not, redirect to login
            User us = db.User.Find(userID);
            if(us.RoleGroup != 2)
            //if (!userID.Substring(0, 4).Equals("0000"))
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new AdminViewModel();
            
            //I think the null should work since the lockout... is optional/nullable?
            var users = from user in db.User where user.LockoutEndDateUtc != null select user;

            viewModel.UnlockableUsers = users;
            viewModel.creationMessage = "";

            

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AdminViewModel viewModel)
        {
            //going to rerun all this, just have the people register as a basic user, then after that, the admin will change their permission group.
            //I run into a bug If I try to make admin user out of someone who has stuff attatched to user id. SO>>>
            // first go through and remove EVERYTHING attatched to user ID. VolunteerPosition both created and a part of, Events, Notifications, then change id.

           


            //I have this so then the model being returned is not null / error prone
            var users = from user in db.User where user.LockoutEndDateUtc != null select user;
            viewModel.UnlockableUsers = users;

            User selectUser = null;

            if (viewModel.NewAdminAccount.Email != null && viewModel.NewAdminAccount.Email.Length > 4)
            {
                //run query to check if email is currently in the system
                var selectedUser = from e in db.User where e.Email.Equals(viewModel.NewAdminAccount.Email) select e;
                foreach (var user in selectedUser)
                {
                    selectUser = user;
                }
            }
            //secondIf to se if we are going back or not
            if (selectUser != null)
            {
                selectUser.RoleGroup = 2;
                db.SaveChanges();
                viewModel.creationMessage = "User: ["+ selectUser.Email +"] is now an Admin User";
                return View(viewModel);
            }
            viewModel.creationMessage = "Not a valid Username/email";


            //return to index if the stuff is invalid?
            return View(viewModel);
        }





        // GET: Admin/Details/5
        public ActionResult Details()
        {
            var viewModel = new AdminViewModel();
            AdminReport report = new AdminReport();

            var events = from eve in db.Event select eve;
            var volunteers = from vol in db.VolunteerPosition select vol;
            var notification = from not in db.Notification select not;

            var filledRoles = 0;
            foreach( var position in volunteers)
            {
                if (position.UserID != null)
                {
                    filledRoles++;
                }
            }

            report.NumEvents = events.Count();
            report.NumVolunteerRoles = volunteers.Count();
            report.NumNotifications = notification.Count();
            report.NumFilledRoles = filledRoles;

            viewModel.report = report;
            viewModel.creationMessage = "";

            return View(viewModel);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Phone,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,RoleGroup")] User user)
        {
            if (ModelState.IsValid)
            {
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Phone,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,RoleGroup")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //should be right?
            User user = db.User.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            user.LockoutEndDateUtc = null;
            db.SaveChanges();
            var newPass = createPass();



            //create their new password and email it to them. http://stackoverflow.com/questions/22516818/how-to-reset-password-with-usermanager-of-asp-net-mvc-5
            // UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            //userManager.RemovePassword(userId);
            //userManager.RemovePassword(id);
            //userManager.AddPassword(id, newPassword);
            //userManager.AddPassword(id, newPass);

            //or http://stackoverflow.com/questions/33684488/failed-to-change-password-with-usermanager-removepassword-and-usermanager-addp

            /*var validPass = await userManager.PasswordValidator.ValidateAsync(newPass);
            if (validPass.Succeeded)
            {
                //var usr = userManager.FindByName(user.Email);
                var usr = userManager.FindById(id);
                user.PasswordHash = userManager.PasswordHasher.HashPassword(newPass);
                var res = userManager.Update(usr);
                if (res.Succeeded)
                {
                    // change password has been succeeded
                }
            }*/

            ForgotPasswordViewModel vm = new ForgotPasswordViewModel();
            vm.Email = user.Email;
            return RedirectToAction("ForgotPassword","Account", new { model = vm });
            //AccountController cont = new AccountController();
            //await cont.ForgotPassword(vm);
            //await cont.CustomResetPassword(id, newPass);



            //or http://stackoverflow.com/questions/19524111/asp-net-identity-reset-password


            //ApplicationDbContext context = new ApplicationDbContext(); //think I already have this set in db?
            /* UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
             UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
             String userId = id;//String userId = User.Identity.GetUserId();//"<YourLogicAssignsRequestedUserId>";
             String newPassword = newPass; //"<PasswordAsTypedByUser>";
             String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
             User cUser = await store.FindByIdAsync(userId);//ApplicationUser cUser = await store.FindByIdAsync(userId);
             await store.SetPasswordHashAsync(cUser, hashedNewPassword);
             await store.UpdateAsync(cUser);*/





            sendMail(user.Email, newPass);

           
            return RedirectToAction("Index", "Admin");
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private string createPass()
        {
            string password = "";
            var lowers = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            var uppers = "ABCDEFGHIJKLMNOPQRSTUVWYXZ".ToCharArray();
            var digits = "0123456789".ToCharArray();
            var symbols = "!@#$%^&*".ToCharArray();
            Random rnd = new Random();
            for (int i = 0; i < 8; i++)
            {

                if (i == 0 || i == 1 || i == 6 || i == 7)
                {
                    int index = rnd.Next(0, lowers.Length);
                    password = password + lowers[index];
                }
                else if (i == 3 || i == 4)
                {
                    int index = rnd.Next(0, uppers.Length);
                    password = password + uppers[index];
                }
                if (i == 2)
                {
                    int index = rnd.Next(0, digits.Length);
                    password = password + digits[index];
                }
                if (i == 5)
                {
                    int index = rnd.Next(0, symbols.Length);
                    password = password + symbols[index];
                }
            }
            return password;
        }
        private void sendMail (string userEmail, string userPassword)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("volunteeringeasy@gmail.com");
                mail.To.Add(userEmail);
                mail.Subject = "VolunteeringEasy Password";
                mail.Body = "Hello,\n\nThe account: " + userEmail + "\nhas had its password reset to: " + userPassword +
                "\n\n The password can be reset upon login by clicking on your username/email in the top right corner of the screen.";
                //mail.IsBodyHtml = true;
                //mail.Attachments.Add(new Attachment("C:\\file.zip"));

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("volunteeringeasy@gmail.com", "51145@Gus");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
