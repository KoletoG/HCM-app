using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HCM_app.ViewModels
{
    public class DepartmentUpdateViewModel
    {
        [Required]
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? JobTitle { get; set; }
        public double? Salary { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public bool ShouldDelete { get; set; }
    }
}
