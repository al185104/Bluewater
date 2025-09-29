using System;
using System.Linq;

namespace Bluewater.App.Models;

public class EmployeeSummary
{
  public Guid Id { get; init; }
  public string FirstName { get; init; } = string.Empty;
  public string LastName { get; init; } = string.Empty;
  public string? MiddleName { get; init; }
  public string? Position { get; init; }
  public string? Section { get; init; }
  public string? Department { get; init; }
  public string? Type { get; init; }
  public string? Level { get; init; }
  public string? Email { get; init; }

  public string FullName
  {
    get
    {
      return string.Join(' ', new[] { FirstName, MiddleName, LastName }
        .Where(part => !string.IsNullOrWhiteSpace(part)));
    }
  }

  public string PositionDisplay => FormatDetail("Position", Position);

  public string DepartmentDisplay => FormatDetail("Department", Department);

  public string SectionDisplay => FormatDetail("Section", Section);

  public string TypeLevelDisplay
  {
    get
    {
      if (string.IsNullOrWhiteSpace(Type) && string.IsNullOrWhiteSpace(Level))
      {
        return "";
      }

      if (string.IsNullOrWhiteSpace(Type))
      {
        return $"Level: {Level}";
      }

      if (string.IsNullOrWhiteSpace(Level))
      {
        return $"Type: {Type}";
      }

      return $"{Type} • {Level}";
    }
  }

  public string EmailDisplay => string.IsNullOrWhiteSpace(Email) ? string.Empty : Email;

  private static string FormatDetail(string label, string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? string.Empty : $"{label}: {value}";
  }
}
