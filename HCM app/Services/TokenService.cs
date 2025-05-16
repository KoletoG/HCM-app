using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HCM_app.Interfaces;

namespace HCM_app.Services
{
    public class TokenService : ITokenService
    {
        public TokenService() 
        {
        }
        public JwtSecurityToken GetToken(byte[] tokenBytes)
        {
            var _handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(tokenBytes);
            return _handler.ReadJwtToken(tokenString);
        }
        public string GetRole(JwtSecurityToken token)
        {
            return token.Claims.First(x => x.Type == ClaimTypes.Role).Value;
        }
        public string GetId(JwtSecurityToken token)
        {
            return token.Claims.First(x => x.Type == "sub").Value;
        }
        public string GetDepartment(JwtSecurityToken token)
        {
            return token.Claims.First(x => x.Type == "Department").Value;
        }
    }
}
