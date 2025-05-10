using SharedModels;

namespace AuthAPIHCM.Interfaces
{
    public interface IAuthService
    {
        public string GenerateJwtToken(UserDataModel user);

    }
}
