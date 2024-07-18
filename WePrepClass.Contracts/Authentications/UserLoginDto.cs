namespace WePrepClass.Contracts.Authentications;

public class UserLoginDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = default!;
}
