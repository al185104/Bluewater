using System.Text;

namespace Bluewater.App.Helpers;

public sealed record SettingsCsvRow(
  string Name,
  string? Description,
  string? Value,
  bool IsActive,
  Guid? ParentId,
  string? ParentName);

public static class SettingsCsvImporter
{
  private static readonly IReadOnlyDictionary<string, string[]> HeaderAliases =
    new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
      ["Name"] = ["Title"],
      ["Description"] = ["Details", "Remarks", "Notes"],
      ["Value"] = ["Code"],
      ["IsActive"] = ["Active", "Enabled"],
      ["ParentId"] = ["ReferenceId", "Parent Id"],
      ["ParentName"] = ["Reference", "Parent", "Parent Name", "Division", "Department", "Section", "Position"]
    };

  public static async Task<IReadOnlyList<SettingsCsvRow>> ParseAsync(
    Stream stream,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(stream);

    using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
    string? headerLine = await reader.ReadLineAsync().ConfigureAwait(false);

    if (string.IsNullOrWhiteSpace(headerLine))
    {
      return Array.Empty<SettingsCsvRow>();
    }

    Dictionary<string, int> headers = CreateHeaderLookup(SplitCsvLine(headerLine));

    if (!ContainsHeader(headers, "Name"))
    {
      throw new FormatException("The CSV file must include a 'Name' column.");
    }

    var rows = new List<SettingsCsvRow>();
    string? line;
    int lineNumber = 1;

    while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
    {
      cancellationToken.ThrowIfCancellationRequested();
      lineNumber++;

      if (string.IsNullOrWhiteSpace(line))
      {
        continue;
      }

      string[] values = SplitCsvLine(line);
      string name = GetValue(values, headers, "Name");

      if (string.IsNullOrWhiteSpace(name))
      {
        continue;
      }

      string? parentIdRaw = GetValue(values, headers, "ParentId");
      Guid? parentId = null;

      if (!string.IsNullOrWhiteSpace(parentIdRaw))
      {
        if (!Guid.TryParse(parentIdRaw, out Guid parsed))
        {
          throw new FormatException($"Line {lineNumber}: '{parentIdRaw}' is not a valid ParentId GUID.");
        }

        parentId = parsed;
      }

      rows.Add(new SettingsCsvRow(
        name.Trim(),
        ToNullIfWhiteSpace(GetValue(values, headers, "Description")),
        ToNullIfWhiteSpace(GetValue(values, headers, "Value")),
        ParseBool(GetValue(values, headers, "IsActive"), defaultValue: true),
        parentId,
        ToNullIfWhiteSpace(GetValue(values, headers, "ParentName"))));
    }

    return rows;
  }

  private static string? ToNullIfWhiteSpace(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

  private static bool ParseBool(string raw, bool defaultValue)
  {
    if (string.IsNullOrWhiteSpace(raw))
    {
      return defaultValue;
    }

    if (bool.TryParse(raw, out bool boolValue))
    {
      return boolValue;
    }

    return raw.Trim().ToLowerInvariant() switch
    {
      "1" or "yes" or "y" => true,
      "0" or "no" or "n" => false,
      _ => defaultValue
    };
  }

  private static bool ContainsHeader(IReadOnlyDictionary<string, int> headers, string canonicalHeader)
  {
    if (headers.ContainsKey(canonicalHeader))
    {
      return true;
    }

    return HeaderAliases.TryGetValue(canonicalHeader, out string[]? aliases)
      && aliases.Any(headers.ContainsKey);
  }

  private static string GetValue(string[] values, IReadOnlyDictionary<string, int> headers, string canonicalHeader)
  {
    if (TryGetHeaderIndex(headers, canonicalHeader, out int index) && index >= 0 && index < values.Length)
    {
      return values[index].Trim();
    }

    return string.Empty;
  }

  private static bool TryGetHeaderIndex(IReadOnlyDictionary<string, int> headers, string canonicalHeader, out int index)
  {
    if (headers.TryGetValue(canonicalHeader, out index))
    {
      return true;
    }

    if (HeaderAliases.TryGetValue(canonicalHeader, out string[]? aliases))
    {
      foreach (string alias in aliases)
      {
        if (headers.TryGetValue(alias, out index))
        {
          return true;
        }
      }
    }

    index = -1;
    return false;
  }

  private static Dictionary<string, int> CreateHeaderLookup(string[] headers)
  {
    var lookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    for (int i = 0; i < headers.Length; i++)
    {
      string header = headers[i].Trim();

      if (!string.IsNullOrWhiteSpace(header) && !lookup.ContainsKey(header))
      {
        lookup[header] = i;
      }
    }

    return lookup;
  }

  private static string[] SplitCsvLine(string line)
  {
    if (string.IsNullOrEmpty(line))
    {
      return Array.Empty<string>();
    }

    var values = new List<string>();
    var current = new StringBuilder();
    bool inQuotes = false;

    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];

      if (c == '"')
      {
        if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
        {
          current.Append('"');
          i++;
        }
        else
        {
          inQuotes = !inQuotes;
        }
      }
      else if (c == ',' && !inQuotes)
      {
        values.Add(current.ToString());
        current.Clear();
      }
      else
      {
        current.Append(c);
      }
    }

    values.Add(current.ToString());
    return values.ToArray();
  }
}
