using FluentValidation;
using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Authentications;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using AuthenticationResult = WePrepClass.Contracts.Authentications.AuthenticationResult;

namespace WePrepClass.Application.UseCases.Accounts.Queries;

public record LoginQuery(
    string Email,
    string Password
) : IQueryRequest<AuthenticationResult>;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}

public class LoginQueryHandler(
    IAppLogger<LoginQueryHandler> logger,
    IMapper mapper,
    IIdentityService identityService,
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator
) : QueryHandlerBase<LoginQuery, AuthenticationResult>(logger, mapper)
{
    public override async Task<Result<AuthenticationResult>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var identityDto = await identityService.SignInAsync(
            request.Email, request.Password);

        if (identityDto is null)
        {
            return Result.Fail(AuthenticationErrorConstants.LoginFailError);
        }

        var customer = await userRepository.GetByCustomerIdAsync(
            UserId.Create(identityDto.Id),
            cancellationToken);

        if (customer is null)
        {
            return Result.Fail(AuthenticationErrorConstants.UserNotFound);
        }

        var loginToken = jwtTokenGenerator.GenerateToken(identityDto);

        var userLoginDto = new UserLoginDto
        {
            Id = customer.Id.Value,
            Email = customer.Email,
            FullName = $"{customer.FirstName} {customer.LastName}",
            Roles = identityDto.Roles,
            Avatar = customer.Avatar
        };

        return new AuthenticationResult(userLoginDto, loginToken);
    }
}