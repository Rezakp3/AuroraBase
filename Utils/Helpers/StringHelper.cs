namespace Utils.Helpers;

public static class StringHelper
{
    public static bool Eq(this string param1, string param2)
        => param1.Trim().Equals(param2.Trim(), StringComparison.CurrentCultureIgnoreCase);
}
