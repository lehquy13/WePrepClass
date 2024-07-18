namespace WePrepClass.Domain.Commons.Enums;

public static class EnumProvider
{
    public static List<string> Roles { get; } = Enum.GetNames(typeof(Role))
        .Where(x => x != Role.All.ToString() && x != Role.Undefined.ToString())
        .ToList();

    public static List<string> Genders => Enum.GetNames(typeof(Gender))
        .Where(x => x != Gender.Both.ToString())
        .ToList();

    public static T ToEnum<T>(this string value) where T : notnull
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}