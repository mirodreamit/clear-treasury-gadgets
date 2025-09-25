using CT.Application.Configuration;
using CT.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CT.Application.Services;

internal class TokenGenerator(ApplicationConfiguration applicationConfiguration) : ITokenGenerator
{
    private readonly ApplicationConfiguration _applicationConfiguration = applicationConfiguration;

    public string CreateAccessToken(List<Claim> claims)
    {
        var expires = DateTime.UtcNow.AddMinutes(_applicationConfiguration.TokenConfiguration.AccessExpirationMins);
        var expiresUnix = new DateTimeOffset(expires).ToUnixTimeSeconds();

        var allClaims = new List<Claim>
        {
            new("created", DateTime.UtcNow.ToString("O")),
            new("expires", expiresUnix.ToString()),
            new("typ", "access")
        };

        allClaims.AddRange(claims);

        return CreateToken(allClaims, expires);
    }

    public string CreateRefreshToken(List<Claim> claims)
    {
        var expires = DateTime.UtcNow.AddDays(_applicationConfiguration.TokenConfiguration.RefreshExpirationDays);
        var expiresUnix = new DateTimeOffset(expires).ToUnixTimeSeconds();

        var allClaims = new List<Claim>
        {
            new("created", DateTime.UtcNow.ToString("O")),
            new("expires", expiresUnix.ToString()),
            new("typ", "refresh")
        };

        allClaims.AddRange(claims);

        return CreateToken(allClaims, expires);
    }

    public JwtSecurityToken DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.ReadJwtToken(token);
    }

    public bool ValidateToken(string token, out SecurityToken? validatedToken)
    {
        validatedToken = null;
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _applicationConfiguration.TokenConfiguration.Issuer,
            ValidAudience = _applicationConfiguration.TokenConfiguration.Audience,
            IssuerSigningKey = GetEncryptionKey()
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string CreateToken(List<Claim> claims, DateTime expires)
    {
        var credentials = new SigningCredentials(GetEncryptionKey(), SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _applicationConfiguration.TokenConfiguration.Issuer,
            audience: _applicationConfiguration.TokenConfiguration.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    private SymmetricSecurityKey GetEncryptionKey()
    {
        if (string.IsNullOrEmpty(_applicationConfiguration.TokenConfiguration.Secret))
        {
            throw new NullReferenceException("TokenConfiguration.Secret cannot be null or empty");
        }

        var bytes = Encoding.UTF8.GetBytes(_applicationConfiguration.TokenConfiguration.Secret);
        
        return new SymmetricSecurityKey(bytes);
    }
}
