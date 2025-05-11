using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HCM_app.ViewModels
{
    public class ProfileViewModel
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
        public string Role { get; set; }
    }
}
