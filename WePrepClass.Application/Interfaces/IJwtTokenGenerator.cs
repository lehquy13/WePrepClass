using System.Security.Claims;
using Matt.SharedKernel.Application.Authorizations;

namespace WePrepClass.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(IdentityDto identityDto);
    IEnumerable<Claim> ValidateToken(string token);
}