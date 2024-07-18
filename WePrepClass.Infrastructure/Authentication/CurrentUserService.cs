using System.Security.Claims;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using WePrepClass.Application.Interfaces;

namespace WePrepClass.Infrastructure.Authentication;

internal class CurrentUserService : ICurrentUserService
{
    private readonly IEnumerable<Claim> _claims = null!;
    private const string PermissionClaimType = "permissions";

    public Guid UserId { get; }
    public List<string> Permissions { get; } = null!;
    public List<string> Roles { get; } = null!;
    public bool IsAuthenticated => UserId != Guid.Empty;
    public string? CurrentUserEmail { get; }
    public string? CurrentUserFullName { get; }

    public string? CurrentTenant { get; }

    public CurrentUserService(
        IJwtTokenGenerator jwtTokenGenerator,
        IHttpContextAccessor httpContextAccessor,
        IAppLogger<CurrentUserService> logger)
    {
        try
        {
            if (httpContextAccessor.HttpContext is null)
            {
                logger.LogInformation("HttpContext is null in CurrentUserService");
                return;
            }

            var token = httpContextAccessor.HttpContext.Request.Headers.Authorization;

            var gettingClaims = jwtTokenGenerator.ValidateToken(
                token.ToString().Split(" ").Last());

            if (gettingClaims.IsSuccess)
            {
                _claims = gettingClaims.Value;
            }
            else
            {
                logger.LogInformation("Getting claims failed: {Message}", gettingClaims.Error);
                return;
            }

            var userId = GetSingleClaimValue(ClaimTypes.NameIdentifier);
            UserId = userId is null ? Guid.Empty : new Guid(userId);

            CurrentUserEmail = GetSingleClaimValue(ClaimTypes.Email);
            CurrentUserFullName = GetSingleClaimValue(ClaimTypes.Name);
            Permissions = GetClaimValues(PermissionClaimType);
            Roles = GetClaimValues(ClaimTypes.Role);
            CurrentTenant = GetSingleClaimValue(ClaimTypes.Actor);
        }
        catch (Exception exception)
        {
            logger.LogInformation("Getting user claims failed: {Message}", exception.Message);
        }
    }

    public void Authenticated()
    {
        if (UserId == Guid.Empty)
        {
            throw new Exception("User is not authenticated");
        }
    }

    private List<string> GetClaimValues(string claimType) =>
        _claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string? GetSingleClaimValue(string claimType) =>
        _claims
            .SingleOrDefault(claim => claim.Type == claimType)?.Value
        ?? null;
}