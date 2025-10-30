using System;
using System.Collections.Generic;
using System.ComponentModel;
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

  private static readonly IReadOnlyDictionary<string, string[]> PossibleHeaderAliases =
    new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
      ["FirstName"] = new[] { "First Name", "Firstname", "GivenName", "Given Name" },
      ["LastName"] = new[] { "Last Name", "Lastname", "Surname", "Family Name" },
      ["MiddleName"] = new[] { "Middle Name", "Middlename", "Middle Initial" },
      ["DateOfBirth"] = new[] { "BirthDate", "Birth Date", "Date of Birth", "DOB" },
      ["Gender"] = new[] { "Sex" },
      ["CivilStatus"] = new[] { "Civil Status", "Marital Status" },
      ["BloodType"] = new[] { "Blood Type", "Blood Group" },
      ["Status"] = new[] { "Employment Status", "Employee Status" },
      ["Remarks"] = new[] { "Comment", "Comments", "Notes" },
      ["Height"] = new[] { "Height (cm)", "HeightCm" },
      ["Weight"] = new[] { "Weight (kg)", "WeightKg" },
      ["Image"] = new[] { "ImageUrl", "Photo", "Profile Image", "ProfilePhoto" },
      ["Tenant"] = new[] { "Project", "Tenant Name" },
      ["MealCredits"] = new[] { "Meal Credit", "Meal Credits", "MealCredit" },
      ["Email"] = new[] { "Email Address", "E-mail", "EmailAddress" },
      ["MobileNumber"] = new[] { "Mobile Number", "MobileNo", "Mobile", "Cell No.", "Cell Number", "Phone Number" },
      ["TelNumber"] = new[] { "Tel Number", "Telephone", "Telephone Number", "Tel No.", "Landline" },
      ["Address"] = new[] { "Address Line", "Home Address", "Residence", "Street Address" },
      ["ProvincialAddress"] = new[] { "Provincial Address", "Province Address" },
      ["MothersMaidenName"] = new[] { "Mother's Maiden Name", "Mothers Maiden Name" },
      ["FathersName"] = new[] { "Father's Name", "Fathers Name" },
      ["EmergencyContact"] = new[] { "Emergency Contact Name", "EmergencyContactName" },
      ["RelationshipContact"] = new[] { "Emergency Relationship", "Relationship" },
      ["AddressContact"] = new[] { "Emergency Contact Address", "EmergencyAddress" },
      ["TelNoContact"] = new[] { "Emergency Tel", "Emergency Telephone", "EmergencyTelNo" },
      ["MobileNoContact"] = new[] { "Emergency Mobile", "Emergency Mobile Number", "EmergencyMobile" },
      ["EducationalAttainment"] = new[] { "Education", "Education Level", "Educational Level" },
      ["CourseGraduated"] = new[] { "Course", "Course Graduated", "Degree" },
      ["UniversityGraduated"] = new[] { "University", "School", "College" },
      ["DateHired"] = new[] { "Hire Date", "Date Hired" },
      ["DateRegularized"] = new[] { "Regularization Date", "Date Regularized" },
      ["DateResigned"] = new[] { "Resignation Date", "Date Resigned" },
      ["DateTerminated"] = new[] { "Termination Date", "Date Terminated" },
      ["TinNo"] = new[] { "TIN", "TIN No", "Tax Identification" },
      ["SssNo"] = new[] { "SSS", "SSS No", "SSS Number" },
      ["HdmfNo"] = new[] { "HDMF", "HDMF No", "Pagibig", "Pag-IBIG Number" },
      ["PhicNo"] = new[] { "PHIC", "Philhealth", "Philhealth Number" },
      ["BankAccount"] = new[] { "Bank Account No", "BankAccountNumber" },
      ["HasServiceCharge"] = new[] { "Service Charge", "ServiceCharge" },
      ["UserId"] = new[] { "User Id", "Employee User Id" },
      ["PositionId"] = new[] { "Position Id", "Employee Position Id" },
      ["PayId"] = new[] { "Pay Id", "Employee Pay Id" },
      ["TypeId"] = new[] { "Type Id", "Employee Type Id" },
      ["LevelId"] = new[] { "Level Id", "Employee Level Id" },
      ["ChargingId"] = new[] { "Charging Id", "Employee Charging Id" }
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

    Tenant tenant = Tenant.Maribago;

    if (TryParseEnum(values, headerLookup, "Tenant", Tenant.Maribago, out Tenant parsedTenant))
    {
      tenant = parsedTenant;
    }

    var request = new CreateEmployeeRequestDto
    {
      FirstName = firstName,
      LastName = lastName,
      MiddleName = NullIfWhiteSpace(GetValue(values, headerLookup, "MiddleName")),
      DateOfBirth = TryParseDate(values, headerLookup, "DateOfBirth"),
      Gender = gender,
      CivilStatus = civilStatus,
      BloodType = bloodType,
      Status = status,
      Remarks = NullIfWhiteSpace(GetValue(values, headerLookup, "Remarks")),
      Height = TryParseDecimal(values, headerLookup, "Height"),
      Weight = TryParseDecimal(values, headerLookup, "Weight"),
      Image = TryParseBase64(values, headerLookup, "Image"),
      Tenant = tenant,
      MealCredits = TryParseInt(values, headerLookup, "MealCredits") ?? 0,
      UserId = TryParseGuid(values, headerLookup, "UserId"),
      PositionId = TryParseGuid(values, headerLookup, "PositionId"),
      PayId = TryParseGuid(values, headerLookup, "PayId"),
      TypeId = TryParseGuid(values, headerLookup, "TypeId"),
      LevelId = TryParseGuid(values, headerLookup, "LevelId"),
      ChargingId = TryParseGuid(values, headerLookup, "ChargingId")
    };

    string? email = GetValue(values, headerLookup, "Email");
    string? mobileNumber = GetValue(values, headerLookup, "MobileNumber");
    string? telNumber = GetValue(values, headerLookup, "TelNumber");
    string? address = GetValue(values, headerLookup, "Address");
    string? provincialAddress = GetValue(values, headerLookup, "ProvincialAddress");
    string? mothersMaidenName = GetValue(values, headerLookup, "MothersMaidenName");
    string? fathersName = GetValue(values, headerLookup, "FathersName");
    string? emergencyContact = GetValue(values, headerLookup, "EmergencyContact");
    string? relationshipContact = GetValue(values, headerLookup, "RelationshipContact");
    string? addressContact = GetValue(values, headerLookup, "AddressContact");
    string? telNoContact = GetValue(values, headerLookup, "TelNoContact");
    string? mobileNoContact = GetValue(values, headerLookup, "MobileNoContact");

    if (!string.IsNullOrWhiteSpace(email) ||
        !string.IsNullOrWhiteSpace(mobileNumber) ||
        !string.IsNullOrWhiteSpace(telNumber) ||
        !string.IsNullOrWhiteSpace(address) ||
        !string.IsNullOrWhiteSpace(provincialAddress) ||
        !string.IsNullOrWhiteSpace(mothersMaidenName) ||
        !string.IsNullOrWhiteSpace(fathersName) ||
        !string.IsNullOrWhiteSpace(emergencyContact) ||
        !string.IsNullOrWhiteSpace(relationshipContact) ||
        !string.IsNullOrWhiteSpace(addressContact) ||
        !string.IsNullOrWhiteSpace(telNoContact) ||
        !string.IsNullOrWhiteSpace(mobileNoContact))
    {
      request.ContactInfo = new CreateEmployeeContactInfoDto
      {
        Email = NullIfWhiteSpace(email),
        MobileNumber = NullIfWhiteSpace(mobileNumber),
        TelNumber = NullIfWhiteSpace(telNumber),
        Address = NullIfWhiteSpace(address),
        ProvincialAddress = NullIfWhiteSpace(provincialAddress),
        MothersMaidenName = NullIfWhiteSpace(mothersMaidenName),
        FathersName = NullIfWhiteSpace(fathersName),
        EmergencyContact = NullIfWhiteSpace(emergencyContact),
        RelationshipContact = NullIfWhiteSpace(relationshipContact),
        AddressContact = NullIfWhiteSpace(addressContact),
        TelNoContact = NullIfWhiteSpace(telNoContact),
        MobileNoContact = NullIfWhiteSpace(mobileNoContact)
      };
    }

    CreateEmployeeEducationInfoDto? educationInfo = BuildEducationInfo(values, headerLookup);

    if (educationInfo is not null)
    {
      request.EducationInfo = educationInfo;
    }

    CreateEmployeeEmploymentInfoDto? employmentInfo = BuildEmploymentInfo(values, headerLookup);

    if (employmentInfo is not null)
    {
      request.EmploymentInfo = employmentInfo;
    }

    return request;
  }

  private static CreateEmployeeEducationInfoDto? BuildEducationInfo(
    string[] values,
    IReadOnlyDictionary<string, int> headerLookup)
  {
    bool hasAttainment = TryParseEnum(
      values,
      headerLookup,
      "EducationalAttainment",
      EducationalAttainment.NotSet,
      out EducationalAttainment attainment) && attainment != EducationalAttainment.NotSet;

    string? courseGraduated = NullIfWhiteSpace(GetValue(values, headerLookup, "CourseGraduated"));
    string? universityGraduated = NullIfWhiteSpace(GetValue(values, headerLookup, "UniversityGraduated"));

    if (!hasAttainment && courseGraduated is null && universityGraduated is null)
    {
      return null;
    }

    return new CreateEmployeeEducationInfoDto
    {
      EducationalAttainment = hasAttainment ? attainment : EducationalAttainment.NotSet,
      CourseGraduated = courseGraduated,
      UniversityGraduated = universityGraduated
    };
  }

  private static CreateEmployeeEmploymentInfoDto? BuildEmploymentInfo(
    string[] values,
    IReadOnlyDictionary<string, int> headerLookup)
  {
    DateTime? dateHired = TryParseDate(values, headerLookup, "DateHired");
    DateTime? dateRegularized = TryParseDate(values, headerLookup, "DateRegularized");
    DateTime? dateResigned = TryParseDate(values, headerLookup, "DateResigned");
    DateTime? dateTerminated = TryParseDate(values, headerLookup, "DateTerminated");
    string? tinNo = NullIfWhiteSpace(GetValue(values, headerLookup, "TinNo"));
    string? sssNo = NullIfWhiteSpace(GetValue(values, headerLookup, "SssNo"));
    string? hdmfNo = NullIfWhiteSpace(GetValue(values, headerLookup, "HdmfNo"));
    string? phicNo = NullIfWhiteSpace(GetValue(values, headerLookup, "PhicNo"));
    string? bankAccount = NullIfWhiteSpace(GetValue(values, headerLookup, "BankAccount"));
    bool? hasServiceCharge = TryParseBool(values, headerLookup, "HasServiceCharge");

    if (!dateHired.HasValue &&
        !dateRegularized.HasValue &&
        !dateResigned.HasValue &&
        !dateTerminated.HasValue &&
        tinNo is null &&
        sssNo is null &&
        hdmfNo is null &&
        phicNo is null &&
        bankAccount is null &&
        hasServiceCharge is null)
    {
      return null;
    }

    return new CreateEmployeeEmploymentInfoDto
    {
      DateHired = dateHired,
      DateRegularized = dateRegularized,
      DateResigned = dateResigned,
      DateTerminated = dateTerminated,
      TinNo = tinNo,
      SssNo = sssNo,
      HdmfNo = hdmfNo,
      PhicNo = phicNo,
      BankAccount = bankAccount,
      HasServiceCharge = hasServiceCharge ?? false
    };
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
        throw new FormatException($"The CSV file must include a '{header}' column.");
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

    if (PossibleHeaderAliases.TryGetValue(name, out string[]? aliases))
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

    if (PossibleHeaderAliases.TryGetValue(name, out string[]? aliases))
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

    string normalizedInput = NormalizeEnumCandidate(value);

    foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
    {
      string normalizedName = NormalizeEnumCandidate(enumValue.ToString());

      if (normalizedInput.Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
      {
        result = enumValue;
        return true;
      }

      string? description = GetEnumDescription(enumValue);

      if (description is not null &&
          normalizedInput.Equals(NormalizeEnumCandidate(description), StringComparison.OrdinalIgnoreCase))
      {
        result = enumValue;
        return true;
      }
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

    if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed))
    {
      return parsed;
    }

    if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out parsed))
    {
      return parsed;
    }

    return null;
  }

  private static int? TryParseInt(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
    {
      return parsed;
    }

    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.CurrentCulture, out parsed))
    {
      return parsed;
    }

    return null;
  }

  private static Guid? TryParseGuid(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    return Guid.TryParse(value, out Guid parsed) ? parsed : null;
  }

  private static bool? TryParseBool(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (bool.TryParse(value, out bool parsed))
    {
      return parsed;
    }

    string normalized = value.Trim().ToLowerInvariant();

    return normalized switch
    {
      "1" or "y" or "yes" or "true" => true,
      "0" or "n" or "no" or "false" => false,
      _ => null
    };
  }

  private static byte[]? TryParseBase64(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    string value = GetValue(values, headers, name);

    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    string trimmed = value.Trim();
    int dataIndex = trimmed.IndexOf(",", StringComparison.Ordinal);

    if (trimmed.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && dataIndex >= 0)
    {
      trimmed = trimmed[(dataIndex + 1)..];
    }

    try
    {
      return Convert.FromBase64String(trimmed);
    }
    catch (FormatException)
    {
      return null;
    }
  }

  private static string? NullIfWhiteSpace(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
  }

  private static string NormalizeEnumCandidate(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return string.Empty;
    }

    var builder = new StringBuilder(value.Length);

    foreach (char c in value)
    {
      if (char.IsLetterOrDigit(c))
      {
        builder.Append(char.ToLowerInvariant(c));
      }
    }

    return builder.ToString();
  }

  private static string? GetEnumDescription<TEnum>(TEnum value)
    where TEnum : struct, Enum
  {
    var member = typeof(TEnum).GetField(value.ToString());

    if (member is null)
    {
      return null;
    }

    return Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute)) is DescriptionAttribute description
      ? description.Description
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
