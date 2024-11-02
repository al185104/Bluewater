using System;
using System.ComponentModel;
using System.Reflection;

namespace Bluewater.UseCases.Helpers;

public static class EnumUtilities
{
    public static List<EnumOption> GetEnumOptions<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>()
            .Select(e => new EnumOption(e, GetEnumDescription(e)))
            .ToList();
    }

    private static string GetEnumDescription<T>(T enumValue) where T : Enum
    {
        FieldInfo? field = enumValue.GetType().GetField(enumValue.ToString());
        DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? enumValue.ToString();
    }

    public static TEnum GetEnumByDescription<TEnum>(string description) where TEnum : Enum
    {
        foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttribute != null && descriptionAttribute.Description == description)
                return enumValue;
        }

        throw new ArgumentException($"No matching enum value found for description: {description}");
    }    
}

public class EnumOption
{
    public Enum? Value { get; set; }
    public string? Description { get; set; }

    public EnumOption(Enum? value, string? description)
    {
        Value = value;
        Description = description;
    }
}