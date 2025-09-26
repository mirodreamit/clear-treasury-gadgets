using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CT.Application.Interfaces;

public interface ITokenGenerator
{
    string CreateAccessToken(List<Claim> claims);
    string CreateRefreshToken(List<Claim> claims);
    bool ValidateToken(string token, out ClaimsPrincipal? principal, out SecurityToken? validatedToken);
    JwtSecurityToken DecodeToken(string token);
}
