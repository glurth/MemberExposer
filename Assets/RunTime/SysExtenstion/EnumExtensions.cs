using System;

public static class EnumExtensions
{
    public static int GetEnumIndex<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        return Array.IndexOf(EnumValues<TEnum>.Values, enumValue);
    }

    private static class EnumValues<TEnum> where TEnum : Enum
    {
        public static readonly TEnum[] Values = (TEnum[])Enum.GetValues(typeof(TEnum));
    }
}
