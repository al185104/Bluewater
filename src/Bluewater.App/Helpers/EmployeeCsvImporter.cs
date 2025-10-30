using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.App.Helpers;

public static class EmployeeCsvImporter
{
  private static readonly string[] RequiredHeaders =
  {
    "FirstName",
    "LastName",
    "Gender",
    "CivilStatus",
    "BloodType",
    "Status"
  };

  public static async Task<IReadOnlyList<CreateEmployeeRequestDto>> ParseAsync(
    Stream stream,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(stream);

    using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
    string? headerLine = await reader.ReadLineAsync().ConfigureAwait(false);

    if (string.IsNullOrWhiteSpace(headerLine))
    {
      return Array.Empty<CreateEmployeeRequestDto>();
    }

    string[] headers = SplitCsvLine(headerLine);
    Dictionary<string, int> headerLookup = CreateHeaderLookup(headers);

    ValidateHeaders(headerLookup);

    var results = new List<CreateEmployeeRequestDto>();
    string? line;

    while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(line))
      {
        continue;
      }

      string[] values = SplitCsvLine(line);
      CreateEmployeeRequestDto? request = ParseRow(values, headerLookup);

      if (request is not null)
      {
        results.Add(request);
      }
    }

    return results;
  }

  private static CreateEmployeeRequestDto? ParseRow(string[] values, IReadOnlyDictionary<string, int> headerLookup)
  {
    string firstName = GetValue(values, headerLookup, "FirstName");
    string lastName = GetValue(values, headerLookup, "LastName");

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
      return null;
    }

    if (!TryParseEnum(values, headerLookup, "Gender", Gender.NotSet, out Gender gender) || gender == Gender.NotSet)
    {
      return null;
    }

    if (!TryParseEnum(values, headerLookup, "CivilStatus", CivilStatus.NotSet, out CivilStatus civilStatus) || civilStatus == CivilStatus.NotSet)
    {
      return null;
    }

    if (!TryParseEnum(values, headerLookup, "BloodType", BloodType.NotSet, out BloodType bloodType) || bloodType == BloodType.NotSet)
    {
      return null;
    }

    if (!TryParseEnum(values, headerLookup, "Status", Status.NotSet, out Status status) || status == Status.NotSet)
    {
      return null;
    }

    var request = new CreateEmployeeRequestDto
    {
      FirstName = firstName,
      LastName = lastName,
      MiddleName = GetValue(values, headerLookup, "MiddleName"),
      DateOfBirth = TryParseDate(values, headerLookup, "DateOfBirth"),
      Gender = gender,
      CivilStatus = civilStatus,
      BloodType = bloodType,
      Status = status,
      Remarks = GetValue(values, headerLookup, "Remarks"),
      Height = TryParseDecimal(values, headerLookup, "Height"),
      Weight = TryParseDecimal(values, headerLookup, "Weight"),
      Tenant = TryParseEnum(values, headerLookup, "Tenant", Tenant.Maribago, out Tenant tenant)
        ? tenant
        : Tenant.Maribago,
      MealCredits = TryParseInt(values, headerLookup, "MealCredits") ?? 0
    };

    string? email = GetValue(values, headerLookup, "Email");
    string? mobileNumber = GetValue(values, headerLookup, "MobileNumber");
    string? telNumber = GetValue(values, headerLookup, "TelNumber");
    string? address = GetValue(values, headerLookup, "Address");

    if (!string.IsNullOrWhiteSpace(email) ||
        !string.IsNullOrWhiteSpace(mobileNumber) ||
        !string.IsNullOrWhiteSpace(telNumber) ||
        !string.IsNullOrWhiteSpace(address))
    {
      request.ContactInfo = new CreateEmployeeContactInfoDto
      {
        Email = string.IsNullOrWhiteSpace(email) ? null : email,
        MobileNumber = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber,
        TelNumber = string.IsNullOrWhiteSpace(telNumber) ? null : telNumber,
        Address = string.IsNullOrWhiteSpace(address) ? null : address
      };
    }

    return request;
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
      if (!headers.ContainsKey(header))
      {
        throw new FormatException($"The CSV file must include a '{header}' column.");
      }
    }
  }

  private static string GetValue(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    return headers.TryGetValue(name, out int index) && index >= 0 && index < values.Length
      ? values[index].Trim()
      : string.Empty;
  }

  private static bool TryParseEnum<TEnum>(
    string[] values,
    IReadOnlyDictionary<string, int> headers,
    string name,
    TEnum defaultValue,
    out TEnum result)
    where TEnum : struct, Enum
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      result = defaultValue;
      return false;
    }

    if (Enum.TryParse(value, ignoreCase: true, out result))
    {
      return true;
    }

    result = defaultValue;
    return false;
  }

  private static DateTime? TryParseDate(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
    {
      return parsed;
    }

    if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
    {
      return parsed;
    }

    return null;
  }

  private static decimal? TryParseDecimal(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed)
      ? parsed
      : null;
  }

  private static int? TryParseInt(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
      ? parsed
      : null;
  }

  private static string[] SplitCsvLine(string line)
  {
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

    for (int i = 0; i < values.Count; i++)
    {
      values[i] = values[i].Trim();
    }

    return values.ToArray();
  }
}
