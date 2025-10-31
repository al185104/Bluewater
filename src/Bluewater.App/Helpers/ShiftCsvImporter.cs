using System.Globalization;
using System.Text;
using Bluewater.App.Models;

namespace Bluewater.App.Helpers;

public static class ShiftCsvImporter
{
  private static readonly string[] RequiredHeaders =
  {
    "Name",
    "ShiftStartTime",
    "ShiftEndTime"
  };

  private static readonly IReadOnlyDictionary<string, string[]> HeaderAliases =
    new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
      ["Name"] = new[] { "ShiftName", "Title" },
      ["ShiftStartTime"] = new[] { "Start", "StartTime", "Shift Start", "Shift Start Time" },
      ["ShiftBreakTime"] = new[] { "BreakStart", "Break Start", "Break Start Time" },
      ["ShiftBreakEndTime"] = new[] { "BreakEnd", "Break End", "Break End Time" },
      ["ShiftEndTime"] = new[] { "End", "EndTime", "Shift End", "Shift End Time" },
      ["BreakHours"] = new[] { "Break Hours", "BreakDuration", "Break Duration" }
    };

  private static readonly IReadOnlyDictionary<string, string> HeaderDisplayNames =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      ["Name"] = "Shift Name",
      ["ShiftStartTime"] = "Shift Start",
      ["ShiftBreakTime"] = "Break Start",
      ["ShiftBreakEndTime"] = "Break End",
      ["ShiftEndTime"] = "Shift End",
      ["BreakHours"] = "Break Hours"
    };

  private static readonly string[] TimeFormats =
  {
    @"hh\:mm",
    @"h\:mm",
    "HH:mm",
    "H:mm"
  };

  public static async Task<IReadOnlyList<ShiftSummary>> ParseAsync(
    Stream stream,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(stream);

    using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
    string? headerLine = await reader.ReadLineAsync().ConfigureAwait(false);

    if (string.IsNullOrWhiteSpace(headerLine))
    {
      return Array.Empty<ShiftSummary>();
    }

    string[] headers = SplitCsvLine(headerLine);
    Dictionary<string, int> headerLookup = CreateHeaderLookup(headers);

    ValidateHeaders(headerLookup);

    var results = new List<ShiftSummary>();
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

      try
      {
        ShiftSummary? shift = ParseRow(values, headerLookup);

        if (shift is not null)
        {
          results.Add(shift);
        }
      }
      catch (FormatException ex)
      {
        throw new FormatException($"Line {lineNumber}: {ex.Message}", ex);
      }
    }

    return results;
  }

  private static ShiftSummary? ParseRow(string[] values, IReadOnlyDictionary<string, int> headers)
  {
    string name = GetValue(values, headers, "Name");

    bool isRowEmpty = string.IsNullOrWhiteSpace(name) &&
                      string.IsNullOrWhiteSpace(GetValue(values, headers, "ShiftStartTime")) &&
                      string.IsNullOrWhiteSpace(GetValue(values, headers, "ShiftEndTime")) &&
                      string.IsNullOrWhiteSpace(GetValue(values, headers, "ShiftBreakTime")) &&
                      string.IsNullOrWhiteSpace(GetValue(values, headers, "ShiftBreakEndTime")) &&
                      string.IsNullOrWhiteSpace(GetValue(values, headers, "BreakHours"));

    if (isRowEmpty)
    {
      return null;
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new FormatException($"The '{GetDisplayName("Name")}' column is required.");
    }

    TimeSpan start = ParseRequiredTime(values, headers, "ShiftStartTime");
    TimeSpan end = ParseRequiredTime(values, headers, "ShiftEndTime");
    TimeSpan? breakStart = ParseOptionalTime(values, headers, "ShiftBreakTime");
    TimeSpan? breakEnd = ParseOptionalTime(values, headers, "ShiftBreakEndTime");
    decimal breakHours = ParseDecimal(values, headers, "BreakHours") ?? 0m;

    var summary = new ShiftSummary
    {
      Id = Guid.Empty,
      Name = name.Trim(),
      BreakHours = breakHours
    };

    summary.ShiftStartTime = FormatTimeSpan(start);
    summary.ShiftEndTime = FormatTimeSpan(end);
    summary.ShiftBreakTime = breakStart.HasValue ? FormatTimeSpan(breakStart.Value) : null;
    summary.ShiftBreakEndTime = breakEnd.HasValue ? FormatTimeSpan(breakEnd.Value) : null;

    return summary;
  }

  private static TimeSpan ParseRequiredTime(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string raw = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(raw))
    {
      throw new FormatException($"The '{GetDisplayName(name)}' column is required.");
    }

    if (TryParseTime(raw, out TimeSpan time))
    {
      return time;
    }

    throw new FormatException($"'{raw}' is not a valid time for '{GetDisplayName(name)}'. Use HH:mm format.");
  }

  private static TimeSpan? ParseOptionalTime(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string raw = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(raw))
    {
      return null;
    }

    if (TryParseTime(raw, out TimeSpan time))
    {
      return time;
    }

    throw new FormatException($"'{raw}' is not a valid time for '{GetDisplayName(name)}'. Use HH:mm format.");
  }

  private static decimal? ParseDecimal(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string raw = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(raw))
    {
      return null;
    }

    if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal invariantResult))
    {
      return invariantResult;
    }

    if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cultureResult))
    {
      return cultureResult;
    }

    throw new FormatException($"'{raw}' is not a valid number for '{GetDisplayName(name)}'.");
  }

  private static Dictionary<string, int> CreateHeaderLookup(string[] headers)
  {
    var lookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    for (int i = 0; i < headers.Length; i++)
    {
      string header = headers[i];

      if (!string.IsNullOrWhiteSpace(header) && !lookup.ContainsKey(header))
      {
        lookup[header] = i;
      }
    }

    return lookup;
  }

  private static void ValidateHeaders(IReadOnlyDictionary<string, int> headers)
  {
    foreach (string header in RequiredHeaders)
    {
      if (!ContainsHeader(headers, header))
      {
        throw new FormatException($"The CSV file must include a '{GetDisplayName(header)}' column.");
      }
    }
  }

  private static string GetValue(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    return TryGetHeaderIndex(headers, name, out int index) && index >= 0 && index < values.Length
      ? values[index].Trim()
      : string.Empty;
  }

  private static bool ContainsHeader(IReadOnlyDictionary<string, int> headers, string name)
  {
    if (headers.ContainsKey(name))
    {
      return true;
    }

    if (HeaderAliases.TryGetValue(name, out string[]? aliases))
    {
      foreach (string alias in aliases)
      {
        if (headers.ContainsKey(alias))
        {
          return true;
        }
      }
    }

    return false;
  }

  private static bool TryGetHeaderIndex(IReadOnlyDictionary<string, int> headers, string name, out int index)
  {
    if (headers.TryGetValue(name, out index))
    {
      return true;
    }

    if (HeaderAliases.TryGetValue(name, out string[]? aliases))
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

  private static bool TryParseTime(string raw, out TimeSpan time)
  {
    string value = raw.Trim();

    foreach (string format in TimeFormats)
    {
      if (TimeSpan.TryParseExact(value, format, CultureInfo.InvariantCulture, out time))
      {
        return true;
      }
    }

    if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out time) ||
        TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out time))
    {
      return true;
    }

    if (TimeOnly.TryParseExact(value, TimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly timeOnly) ||
        TimeOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out timeOnly) ||
        TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeOnly))
    {
      time = timeOnly.ToTimeSpan();
      return true;
    }

    time = default;
    return false;
  }

  private static string GetDisplayName(string name)
  {
    return HeaderDisplayNames.TryGetValue(name, out string? display)
      ? display
      : name;
  }

  private static string FormatTimeSpan(TimeSpan value)
  {
    return value.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
  }

  private static string[] SplitCsvLine(string line)
  {
    var values = new List<string>();
    var current = new StringBuilder();
    bool inQuotes = false;

    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];

      if (c == '\"')
      {
        if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
        {
          current.Append('\"');
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

    for (int i = 0; i < values.Count; i++)
    {
      values[i] = values[i].Trim();
    }

    return values.ToArray();
  }
}
