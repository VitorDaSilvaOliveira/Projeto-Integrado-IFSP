using System.Reflection;

namespace Estoque.Infrastructure.Utils;

public static class ObjectUtils
{
    public static string? SafeGetString(object obj, string propName)
    {
        var prop = obj?.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null) return null;
        var val = prop.GetValue(obj);
        return val?.ToString();
    }

    public static object SafeGetObject(object obj, string propName)
    {
        var prop = obj?.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null) return null;
        return prop.GetValue(obj);
    }

    public static int SafeGetInt(object obj, string propName)
    {
        var v = SafeGetObject(obj, propName);
        if (v == null) return 0;
        if (v is int i) return i;
        if (int.TryParse(v.ToString(), out var r)) return r;
        return 0;
    }

    public static decimal SafeGetDecimal(object obj, string propName)
    {
        var v = SafeGetObject(obj, propName);
        if (v == null) return 0m;
        if (v is decimal d) return d;
        if (v is double dbl) return Convert.ToDecimal(dbl);
        if (decimal.TryParse(v.ToString(), out var r)) return r;
        return 0m;
    }
}
