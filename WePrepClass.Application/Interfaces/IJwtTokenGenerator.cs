using System.Security.Claims;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Authorizations;

namespace WePrepClass.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(IdentityDto identityDto);
    Result<IEnumerable<Claim>> ValidateToken(string token);
}