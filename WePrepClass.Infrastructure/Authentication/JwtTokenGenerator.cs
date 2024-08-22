using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matt.ResultObject;
using Matt.SharedKernel;
using Matt.SharedKernel.Application.Authorizations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WePrepClass.Application.Interfaces;
using WePrepClass.Infrastructure.Models;

namespace WePrepClass.Infrastructure.Authentication;

internal class JwtTokenGenerator(IOptions<JwtSettings> options) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public string GenerateToken(IdentityDto identityDto)
    {
        var signingCredential = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)
            ),
            SecurityAlgorithms.HmacSha256
        );

        List<Claim> claims =
        [
            new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, identityDto.Id.ToString())
        ];

        identityDto.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

        if (identityDto.Tenant is not null)
        {
            claims.Add(new Claim(ClaimTypes.Actor, $"{identityDto.Tenant}.{identityDto.Id.ToString()}"));
        }

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredential
        );
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public Result<IEnumerable<Claim>> ValidateToken(string token)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true
                // ClockSkew = TimeSpan.Zero // zero tolerance for the token lifetime expiration time
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // token is expired, redirect to authentication page
            // token is still valid, navigate to home page
            return IsTokenExpired(jwtToken)
                ? Result.Fail("Token is expired")
                : Result<IEnumerable<Claim>>.Success(jwtToken.Claims);
        }
        catch
        {
            return Result.Fail("Token is invalid");
        }
    }

    private static bool IsTokenExpired(SecurityToken jwtToken)
    {
        return jwtToken.ValidTo < DateTime.Now;
    }
}