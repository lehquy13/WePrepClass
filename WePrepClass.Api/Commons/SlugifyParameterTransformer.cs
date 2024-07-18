using System.Text.RegularExpressions;

namespace WePrepClass.Api.Commons;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null)
            return null;

        var str = value.ToString();

        return string.IsNullOrEmpty(str)
            ? null
            : RegexGenerator.SlugifyParameterRegex().Replace(str, "$1-$2").ToLower();
    }
}

public static partial class RegexGenerator
{
    [GeneratedRegex("([a-z])([A-Z])", RegexOptions.Compiled)]
    public static partial Regex SlugifyParameterRegex();
}