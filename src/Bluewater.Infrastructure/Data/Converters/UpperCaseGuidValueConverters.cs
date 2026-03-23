using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bluewater.Infrastructure.Data.Converters;

internal sealed class UpperCaseGuidToStringConverter() : ValueConverter<Guid, string>(
  guid => guid.ToString("D").ToUpperInvariant(),
  value => Guid.Parse(value))
{
}

internal sealed class NullableUpperCaseGuidToStringConverter() : ValueConverter<Guid?, string?>(
  guid => guid.HasValue ? guid.Value.ToString("D").ToUpperInvariant() : null,
  value => string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value))
{
}
