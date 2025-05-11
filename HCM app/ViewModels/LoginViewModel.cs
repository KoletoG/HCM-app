using System.ComponentModel.DataAnnotations;

namespace HCM_app.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name ="Password")]
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
