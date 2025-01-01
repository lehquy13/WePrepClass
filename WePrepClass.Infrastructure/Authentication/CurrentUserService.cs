using System.Security.Claims;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WePrepClass.Application.Interfaces;

namespace WePrepClass.Infrastructure.Authentication;

internal class CurrentUserService : ICurrentUserService
{
    private readonly IEnumerable<Claim> _claims = null!;
    private const string PermissionClaimType = "permissions";

    public List<string> Permissions => GetClaimValues(PermissionClaimType);
    public List<string> Roles => GetClaimValues(ClaimTypes.Role);
    public bool IsAuthenticated => UserId != Guid.Empty;

    public Guid UserId
    {
        get
        {
            var userId = GetSingleClaimValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrEmpty(userId)
                ? Guid.Empty
                : Guid.Parse(userId);
        }
    }

    public string Email => GetSingleClaimValue(ClaimTypes.Email);
    public string FullName => GetSingleClaimValue(ClaimTypes.Name);
    public string Tenant => GetSingleClaimValue(ClaimTypes.Actor);

    public CurrentUserService(
        IJwtTokenGenerator jwtTokenGenerator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CurrentUserService> logger)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            logger.LogInformation("HttpContext is null in CurrentUserService");

            return;
        }

        var token = httpContextAccessor.HttpContext.Request.Headers.Authorization;

        _claims = jwtTokenGenerator.ValidateToken(token.ToString().Split(" ").Last());
    }

    public Result Authenticated() => UserId == Guid.Empty ? Result.Unauthorized() : Result.Success();

    private List<string> GetClaimValues(string claimType) =>
        _claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string GetSingleClaimValue(string claimType) =>
        _claims.SingleOrDefault(claim => claim.Type == claimType)?.Value ?? string.Empty;
}