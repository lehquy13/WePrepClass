// ReSharper disable NotAccessedPositionalProperty.Global

namespace WePrepClass.Contracts.Authentications;

public record AuthenticationResult(UserLoginDto User, string Token);