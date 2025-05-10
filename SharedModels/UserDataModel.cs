using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SharedModels
{
    public class UserDataModel
    {
        [Key]
        public string Id { get; set; }
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
        [NotMapped]
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public UserDataModel()
        {

        }
    }
    public enum UserRole
    {
        Employee,
        Manager,
        HrAdmin
    }
}
