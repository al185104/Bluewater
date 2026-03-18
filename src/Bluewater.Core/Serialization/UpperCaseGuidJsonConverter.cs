using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bluewater.Core.Serialization;

public sealed class UpperCaseGuidJsonConverter : JsonConverter<Guid>
{
  public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
    {
      throw new JsonException($"Expected {JsonTokenType.String} but found {reader.TokenType}.");
    }

    string? value = reader.GetString();
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new JsonException("GUID value cannot be null or empty.");
    }

    if (Guid.TryParse(value, out Guid guid))
    {
      return guid;
    }

    throw new JsonException($"'{value}' is not a valid GUID.");
  }

  public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToString("D").ToUpperInvariant());
  }
}

public sealed class NullableUpperCaseGuidJsonConverter : JsonConverter<Guid?>
{
  public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType == JsonTokenType.Null)
    {
      return null;
    }

    if (reader.TokenType != JsonTokenType.String)
    {
      throw new JsonException($"Expected {JsonTokenType.String} but found {reader.TokenType}.");
    }

    string? value = reader.GetString();
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (Guid.TryParse(value, out Guid guid))
    {
      return guid;
    }

    throw new JsonException($"'{value}' is not a valid GUID.");
  }

  public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
  {
    if (!value.HasValue)
    {
      writer.WriteNullValue();
      return;
    }

    writer.WriteStringValue(value.Value.ToString("D").ToUpperInvariant());
  }
}

public static class JsonSerializerOptionsGuidExtensions
{
  public static JsonSerializerOptions UseUpperCaseGuids(this JsonSerializerOptions options)
  {
    options.Converters.Add(new UpperCaseGuidJsonConverter());
    options.Converters.Add(new NullableUpperCaseGuidJsonConverter());
    return options;
  }
}
