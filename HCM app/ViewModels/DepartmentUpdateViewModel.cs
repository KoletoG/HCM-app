using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HCM_app.ViewModels
{
    public class DepartmentUpdateViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public double Salary { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
