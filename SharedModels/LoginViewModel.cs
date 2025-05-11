using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class LoginViewModel
    {
        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public LoginViewModel()
        {

        }
    }
}
