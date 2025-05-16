using HCM_app.ViewModels;

namespace HCM_app.Interfaces
{
    public interface IUserInputService
    {
        public void SanitizeInput(List<DepartmentUpdateViewModel> viewModel);
        public void ChangeRoleNaming(List<DepartmentUpdateViewModel> users);
        public bool IsValidSalary(List<DepartmentUpdateViewModel> users);
        public bool HasInvalidRole(List<DepartmentUpdateViewModel> users);
    }
}
