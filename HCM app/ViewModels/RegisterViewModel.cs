using SharedModels;
using System.ComponentModel.DataAnnotations;

namespace HCM_app.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string JobTitle { get; set; }
        [DataType(DataType.Currency)]
        public double Salary { get; set; }
        public string Department { get; set; }
        [Required]
        public string Password { get; set; }
        public RegisterViewModel()
        {

        }
    }
}
