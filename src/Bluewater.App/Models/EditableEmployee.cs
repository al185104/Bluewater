using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.Models;

public partial class EditableEmployee : ObservableObject
{
  public Guid Id { get; set; }

  [ObservableProperty]
  private string firstName = string.Empty;

  [ObservableProperty]
  private string lastName = string.Empty;

  [ObservableProperty]
  private string? middleName;

  [ObservableProperty]
  private string? position;

  [ObservableProperty]
  private string? department;

  [ObservableProperty]
  private string? section;

  [ObservableProperty]
  private string? type;

  [ObservableProperty]
  private string? level;

  [ObservableProperty]
  private string? email;

  [ObservableProperty]
  private string? image;

  public string FullName
  {
    get
    {
      return string.Join(' ', new[] { FirstName, MiddleName, LastName }
        .Where(part => !string.IsNullOrWhiteSpace(part)));
    }
  }

  public static EditableEmployee FromSummary(EmployeeSummary summary)
  {
    return new EditableEmployee
    {
      Id = summary.Id,
      FirstName = summary.FirstName,
      LastName = summary.LastName,
      MiddleName = summary.MiddleName,
      Position = summary.Position,
      Department = summary.Department,
      Section = summary.Section,
      Type = summary.Type,
      Level = summary.Level,
      Email = summary.Email,
      Image = summary.Image
    };
  }

  public EmployeeSummary ToSummary(int rowIndex)
  {
    return new EmployeeSummary
    {
      Id = Id,
      FirstName = FirstName,
      LastName = LastName,
      MiddleName = MiddleName,
      Position = Position,
      Department = Department,
      Section = Section,
      Type = Type,
      Level = Level,
      Email = Email,
      Image = Image,
      RowIndex = rowIndex
    };
  }
}
