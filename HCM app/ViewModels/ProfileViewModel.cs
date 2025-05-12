using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HCM_app.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Job title")]
        public string JobTitle { get; set; }
        [DataType(DataType.Currency)]
        [DisplayName("Salary")]
        public double Salary { get; set; }
        [DisplayName("Department")]
        public string Department { get; set; }
        [DisplayName("Role")]
        public string Role { get; set; }
    }
}
