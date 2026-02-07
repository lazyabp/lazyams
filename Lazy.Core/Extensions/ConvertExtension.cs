using System.Collections;

namespace Lazy.Core.Extensions;

public static class ConvertExtension
{
    #region 转换为long
    /// <summary>
    /// 将object转换为long，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long ParseToLong(this object obj)
    {
        if (obj == null)
            return 0L;

        var state = long.TryParse(obj.ToString(), out var value);
        if (state)
            return value;
        else
            return 0L;
    }

    /// <summary>
    /// 将object转换为long，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ParseToLong(this string str, long defaultValue)
    {
        if (!string.IsNullOrEmpty(str))
            return defaultValue;

        var state = long.TryParse(str, out var value);
        if (state)
            return value;
        else
            return defaultValue;
    }

    #endregion

    #region 转换为int
    /// <summary>
    /// 将object转换为int，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int ParseToInt(this object str)
    {
        if (str == null)
            return 0;

        try
        {
            return Convert.ToInt32(str);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 将object转换为int，若转换失败，则返回指定值。不抛出异常。 
    /// null返回默认值
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ParseToInt(this object str, int defaultValue)
    {
        if (str == null)
            return defaultValue;

        try
        {
            return Convert.ToInt32(str);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion

    #region 转换为short
    /// <summary>
    /// 将object转换为short，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static short ParseToShort(this object obj)
    {
        if (obj == null)
            return 0;

        var state = short.TryParse(obj.ToString(), out var value);
        if (state)
            return value;
        else
            return 0;
    }

    /// <summary>
    /// 将object转换为short，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static short ParseToShort(this object str, short defaultValue)
    {
        if (str == null)
            return defaultValue;

        var state = short.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return defaultValue;
    }

    #endregion

    #region 转换为demical
    /// <summary>
    /// 将object转换为demical，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ParseToDecimal(this object str, decimal defaultValue)
    {
        if (str == null)
            return defaultValue;

        var state = decimal.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return defaultValue;
    }

    /// <summary>
    /// 将object转换为demical，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ParseToDecimal(this object str)
    {
        if (str == null)
            return 0;

        var state = decimal.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return 0;
    }

    #endregion

    #region 转化为bool
    /// <summary>
    /// 将object转换为bool，若转换失败，则返回false。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool ParseToBool(this object str)
    {
        if (str == null)
            return false;

        var state = bool.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return false;
    }

    /// <summary>
    /// 将object转换为bool，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool ParseToBool(this object str, bool defaultValue)
    {
        if (str == null)
            return defaultValue;

        var state = bool.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return defaultValue;
    }

    #endregion

    #region 转换为float
    /// <summary>
    /// 将object转换为float，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float ParseToFloat(this object str)
    {
        if (str == null)
            return 0f;

        var state = float.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return 0f;
    }

    /// <summary>
    /// 将object转换为float，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float ParseToFloat(this object str, float defaultValue)
    {
        if (str == null)
            return defaultValue;

        var state = float.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return defaultValue;
    }

    #endregion

    #region 转换为Guid
    /// <summary>
    /// 将string转换为Guid，若转换失败，则返回Guid.Empty。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Guid ParseToGuid(this string str)
    {
        if (str == null)
            return Guid.Empty;

        var state = Guid.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return Guid.Empty;
    }

    #endregion

    #region 转换为DateTime
    /// <summary>
    /// 将string转换为DateTime，若转换失败，则返回日期最小值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return DateTime.MinValue;
            }
            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }
            else
            {
                int length = str.Length;
                switch (length)
                {
                    case 4:
                        return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    case 6:
                        return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                    case 8:
                        return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    case 10:
                        return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                    case 12:
                        return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                    case 14:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    default:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                }
            }
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// 将string转换为DateTime，若转换失败，则返回默认值。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str, DateTime? defaultValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue.GetValueOrDefault();
            }
            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }
            else
            {
                int length = str.Length;
                switch (length)
                {
                    case 4:
                        return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    case 6:
                        return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                    case 8:
                        return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    case 10:
                        return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                    case 12:
                        return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                    case 14:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    default:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                }
            }
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }
    #endregion

    #region 转换为string
    /// <summary>
    /// 将object转换为string，若转换失败，则返回""。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ParseToString(this object obj)
    {
        try
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToString();
            }
        }
        catch
        {
            return string.Empty;
        }
    }
    public static string ParseToStrings<T>(this object obj)
    {
        try
        {
            var list = obj as IEnumerable<T>;
            if (list != null)
            {
                return string.Join(",", list);
            }
            else
            {
                return obj.ToString();
            }
        }
        catch
        {
            return string.Empty;
        }

    }
    #endregion

    #region 转换为double
    /// <summary>
    /// 将object转换为double，若转换失败，则返回0。不抛出异常。  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object obj)
    {
        if (obj == null)
            return 0d;

        var state = double.TryParse(obj.ToString(), out var value);
        if (state)
            return value;
        else
            return 0d;
    }

    /// <summary>
    /// 将object转换为double，若转换失败，则返回指定值。不抛出异常。  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object str, double defaultValue)
    {
        if (str == null)
            return defaultValue;

        var state = double.TryParse(str.ToString(), out var value);
        if (state)
            return value;
        else
            return defaultValue;
    }

    #endregion

    #region 强制转换类型
    /// <summary>
    /// 强制转换类型
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> CastSuper<TResult>(this IEnumerable source)
    {
        foreach (object item in source)
        {
            yield return (TResult)Convert.ChangeType(item, typeof(TResult));
        }
    }

    /// <summary>
    /// Converts given object to a value or enum type using <see cref="Convert.ChangeType(object, TypeCode)"/> or <see cref="Enum.Parse(Type, string)"/> method.
    /// </summary>
    /// <param name="obj">Object to be converted</param>
    /// <typeparam name="T">Type of the target object</typeparam>
    /// <returns>Converted object</returns>
    public static T To<T>(this object obj) where T : struct
    {
        if (typeof(T) == typeof(Guid))
        {
            return (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
        }

        if (typeof(T).IsEnum)
        {
            if (System.Enum.IsDefined(typeof(T), obj))
            {
                return (T)System.Enum.Parse(typeof(T), obj.ToString());
            }
            else
            {
                throw new ArgumentException($"Enum type undefined '{obj}'.");
            }
        }

        return (T)Convert.ChangeType(obj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
    }

    #endregion
}
