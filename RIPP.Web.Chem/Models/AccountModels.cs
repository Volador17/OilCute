using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace RIPP.Web.Chem.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "邮箱或手机号")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
      

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码确认")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string rePassword { get; set; }

        [Required]
        [Display(Name = "真实名")]
        public string RealName { get; set; }

        [Display(Name = "手机号")]
        public string CellPhone { get; set; }

        [Display(Name = "实验室")]
        public int GroupID { get; set; }

        public int U_Type { set; get; }
    }


    public class AccountHelper
    {

        public static bool ValidateUser(string UserName, string Password)
        {
          
            return true;
        }
    }
}
