using Ganss.Xss;
using HCM_app.Interfaces;
using HCM_app.ViewModels;

namespace HCM_app.Services
{
    public class UserInputService : IUserInputService
    {
        private readonly IHtmlSanitizer _htmlSanitizer;
        public UserInputService(IHtmlSanitizer sanitizer)
        {
            _htmlSanitizer = sanitizer;
        }
        public void SanitizeInput(List<DepartmentUpdateViewModel> viewModel)
        {
            try
            {
                foreach (var user in viewModel)
                {
                    user.Id = _htmlSanitizer.Sanitize(user.Id);
                    user.Department = _htmlSanitizer.Sanitize(user.Department);
                    user.JobTitle = _htmlSanitizer.Sanitize(user.JobTitle);
                    user.Email = _htmlSanitizer.Sanitize(user.Email);
                    user.FirstName = _htmlSanitizer.Sanitize(user.FirstName);
                    user.LastName = _htmlSanitizer.Sanitize(user.LastName);
                    user.Role = _htmlSanitizer.Sanitize(user.Role);
                }
            }
            catch (Exception)
            {
            }
        }
        public void ChangeRoleNaming(List<DepartmentUpdateViewModel> users)
        {
            foreach (var user in users)
            {
                string userRoleLowered = user.Role.ToLower();
                if (userRoleLowered == "manager")
                {
                    user.Role = "Manager";
                }
                else if (userRoleLowered == "hradmin")
                {
                    user.Role = "HrAdmin";
                }
                else if (userRoleLowered == "employee")
                {
                    user.Role = "Employee";
                }
            }
        }
        public bool IsValidSalary(List<DepartmentUpdateViewModel> users)
        {
            foreach (var user in users)
            {
                if (user.Salary != default)
                {
                    if (user.Salary < 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool HasInvalidRole(List<DepartmentUpdateViewModel> users)
        {
            foreach (var user in users)
            {
                if (user.Role != "Manager" && user.Role != "HrAdmin" && user.Role != "Employee" && !string.IsNullOrEmpty(user.Role))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
