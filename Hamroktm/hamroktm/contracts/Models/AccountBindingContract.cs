using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contracts.Models
{
    public class AccountBindingContract
    {

        public class AddExternalLoginBindingModel
        {
            [Required]
            [Display(Name = "External access token")]
            public string ExternalAccessToken { get; set; }
        }


        //Password Related
        public class ChangePasswordBindingContractModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public class ResetPasswordContractModel
        {
            //[Required]
            //[EmailAddress]
            //[Display(Name = "Email")]
            //public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
            public string UserId { get; set; }
        }


        public class ForgotPasswordContractModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            public string Code { get; set; }
        }

        public class ConfirmationContractModel
        {
            public string UserId { get; set; }
            public string Email { get; set; }

            public string Code { get; set; }
        }

        public class SendEmail
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
        }


        public class RegisterBindingModel
        {
            [Required]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public class RegisterExternalBindingModel
        {
            [Required]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

        public class RemoveLoginBindingModel
        {
            [Required]
            [Display(Name = "Login provider")]
            public string LoginProvider { get; set; }

            [Required]
            [Display(Name = "Provider key")]
            public string ProviderKey { get; set; }
        }

        public class SetPasswordBindingModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public class CheckEmailConfirmation
        {
            public string UserName { get; set; }
        }
    }
}
