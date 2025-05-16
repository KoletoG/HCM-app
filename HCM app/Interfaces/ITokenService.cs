using System.IdentityModel.Tokens.Jwt;

namespace HCM_app.Interfaces
{
    public interface ITokenService
    {
        public JwtSecurityToken GetToken(byte[] tokenBytes);

        public string GetRole(JwtSecurityToken token);

        public string GetId(JwtSecurityToken token);
        public string GetDepartment(JwtSecurityToken token);
    }
}
