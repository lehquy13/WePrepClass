namespace WePrepClass.Domain.Commons.Enums;

public static class EnumProvider
{
    public static List<string> Roles { get; } = Enum.GetNames(typeof(Role)).ToList();
    public static List<string> Genders => Enum.GetNames(typeof(Gender)).ToList();

    public static T ToEnum<T>(this string value) where T : notnull
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}