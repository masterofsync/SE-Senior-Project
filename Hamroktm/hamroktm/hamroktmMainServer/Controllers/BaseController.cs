using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using contracts.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace hamroktmMainServer.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected BaseController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;        
        }

        protected BaseController()
        {
             
        }
        private ApplicationUserManager _userManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        //PUT api/Account/SendEmailForgottenPassword
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("SendEmail")]
        public IHttpActionResult SendEmail(AccountBindingContract.SendEmail model)
        {
            if (model != null)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(model.Email);
                mail.From = new MailAddress("easilybuyandsell@gmail.com");
                mail.Subject = model.Subject;
                string Body = model.Content;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                    ("easilybuyandsell@gmail.com", "mynameisbikesh"); // Enter seders User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return Ok();
            }
            return BadRequest("Model is null!");
        }


    }
}