using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bluewater.App.Helpers;

public static class ScheduleCsvImporter
{
  private static readonly string[] RequiredHeaders =
  {
    "EmployeeId",
    "ScheduleDate",
    "ShiftId"
  };

  private static readonly IReadOnlyDictionary<string, string[]> HeaderAliases =
    new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
      ["ScheduleId"] = new[] { "Id", "Schedule Id" },
      ["EmployeeId"] = new[] { "Employee Id" },
      ["ShiftId"] = new[] { "Shift Id" },
      ["ScheduleDate"] = new[] { "Date", "Schedule Date" },
      ["IsDefault"] = new[] { "Default", "Is Default" },
      ["ShiftName"] = new[] { "Shift Name", "Name" }
    };

  private static readonly IReadOnlyDictionary<string, string> HeaderDisplayNames =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      ["ScheduleId"] = "Schedule Id",
      ["EmployeeId"] = "Employee Id",
      ["ShiftId"] = "Shift Id",
      ["ScheduleDate"] = "Schedule Date",
      ["IsDefault"] = "Is Default",
      ["ShiftName"] = "Shift Name"
    };

  private static readonly string[] DateFormats =
  {
    "yyyy-MM-dd",
    "MM/dd/yyyy",
    "M/d/yyyy",
    "dd/MM/yyyy",
    "d/M/yyyy"
  };

  public static async Task<IReadOnlyList<ScheduleCsvRecord>> ParseAsync(
    Stream stream,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(stream);

    using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
    string? headerLine = await reader.ReadLineAsync().ConfigureAwait(false);

    if (string.IsNullOrWhiteSpace(headerLine))
    {
      return Array.Empty<ScheduleCsvRecord>();
    }

    string[] headers = SplitCsvLine(headerLine);
    Dictionary<string, int> headerLookup = CreateHeaderLookup(headers);

    ValidateHeaders(headerLookup);

    var results = new List<ScheduleCsvRecord>();
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
        ScheduleCsvRecord? record = ParseRow(values, headerLookup);

        if (record is not null)
        {
          results.Add(record);
        }
      }
      catch (FormatException ex)
      {
        throw new FormatException($"Line {lineNumber}: {ex.Message}", ex);
      }
    }

    return results;
  }

  private static ScheduleCsvRecord? ParseRow(string[] values, IReadOnlyDictionary<string, int> headers)
  {
    string employeeValue = GetValue(values, headers, "EmployeeId");
    string dateValue = GetValue(values, headers, "ScheduleDate");
    string shiftValue = GetValue(values, headers, "ShiftId");
    string scheduleValue = GetValue(values, headers, "ScheduleId");

    bool isRowEmpty = string.IsNullOrWhiteSpace(employeeValue) &&
                      string.IsNullOrWhiteSpace(dateValue) &&
                      string.IsNullOrWhiteSpace(shiftValue) &&
                      string.IsNullOrWhiteSpace(scheduleValue);

    if (isRowEmpty)
    {
      return null;
    }

    Guid employeeId = ParseRequiredGuid(employeeValue, "EmployeeId");
    Guid shiftId = ParseOptionalGuid(shiftValue) ?? Guid.Empty;
    DateOnly scheduleDate = ParseRequiredDate(dateValue, "ScheduleDate");
    Guid? scheduleId = ParseOptionalGuid(scheduleValue);
    bool isDefault = ParseBoolean(GetValue(values, headers, "IsDefault")) ?? false;
    string shiftNameValue = GetValue(values, headers, "ShiftName");
    string? shiftName = string.IsNullOrWhiteSpace(shiftNameValue) ? null : shiftNameValue;

    return new ScheduleCsvRecord(scheduleId, employeeId, shiftId, scheduleDate, isDefault, shiftName);
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

  private static Guid ParseRequiredGuid(string value, string name)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new FormatException($"The '{GetDisplayName(name)}' column is required.");
    }

    if (Guid.TryParse(value, out Guid result))
    {
      return result;
    }

    throw new FormatException($"'{value}' is not a valid GUID for '{GetDisplayName(name)}'.");
  }

  private static Guid? ParseOptionalGuid(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    return Guid.TryParse(value, out Guid result) ? result : throw new FormatException($"'{value}' is not a valid GUID.");
  }

  private static DateOnly ParseRequiredDate(string value, string name)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new FormatException($"The '{GetDisplayName(name)}' column is required.");
    }

    if (DateOnly.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date) ||
        DateOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out date) ||
        DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
    {
      return date;
    }

    throw new FormatException($"'{value}' is not a valid date for '{GetDisplayName(name)}'.");
  }

  private static bool? ParseBoolean(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (bool.TryParse(value, out bool boolResult))
    {
      return boolResult;
    }

    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intResult))
    {
      return intResult != 0;
    }

    string normalized = value.Trim();

    if (string.Equals(normalized, "yes", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(normalized, "y", StringComparison.OrdinalIgnoreCase))
    {
      return true;
    }

    if (string.Equals(normalized, "no", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(normalized, "n", StringComparison.OrdinalIgnoreCase))
    {
      return false;
    }

    throw new FormatException($"'{value}' is not a valid boolean value.");
  }

  private static string GetDisplayName(string name)
  {
    return HeaderDisplayNames.TryGetValue(name, out string? display)
      ? display
      : name;
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

public record class ScheduleCsvRecord(
  Guid? ScheduleId,
  Guid EmployeeId,
  Guid ShiftId,
  DateOnly ScheduleDate,
  bool IsDefault,
  string? ShiftName);
