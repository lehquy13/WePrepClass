// ReSharper disable ReplaceAutoPropertyWithComputedProperty
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace WePrepClass.Infrastructure.Models;

internal class EmailSettingNames
{
    public static string SectionName { get; } = "EmailSettingNames";
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool EnableSsl { get; init; }
    public string? SmtpClient { get; init; } = null!;
    public int Port { get; init; }
    public bool UseDefaultCredentials { get; init; }
}