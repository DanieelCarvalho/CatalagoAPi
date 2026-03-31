using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CatalogoApi.Service;

public class TokenService : ITokenService
{
    public JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims, 
                                               IConfiguration _config)
    {
        var key = _config.GetSection("Jwt").GetValue<string>("SecretKey") ??
                   throw new InvalidOperationException("Invalid secret Key");

        var privateKey = Encoding.UTF8.GetBytes(key);

        var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(privateKey),
                SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT")
                     .GetValue<double>("TokenValidityInMinutes")),
            Audience = _config.GetSection("Jwt").GetValue<string>("ValidAudience"),
            Issuer = _config.GetSection("Jwt").GetValue<string>("ValidIssuer"),
            SigningCredentials = signingCredentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        var secureRandomBytes = new byte[128];

        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(secureRandomBytes);

        var refreshToken = Convert.ToBase64String(secureRandomBytes);

       return refreshToken;
    }

    public ClaimsPrincipal GetPrincipalFromExpeiredToken(string token,
                                                         IConfiguration _config)
    {
        var secretKey = _config["Jwt:SecretKey"] ??
            throw new InvalidOperationException("Invalid key!!");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer =    false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, 
                                                              out var securityToken);

        if( securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                                               StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token!!");
        }

        return principal;
    }
}
