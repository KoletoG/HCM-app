using System.ComponentModel.DataAnnotations;

namespace HCM_app.Models.DataModels
{
    public class UserDataModel
    {
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public double Salary { get; set; }
        public string Department { get; set; }
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
