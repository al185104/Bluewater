using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.Models;

public partial class EditableEmployee : ObservableObject
{
  public Guid Id { get; set; }

  [ObservableProperty]
  public partial string FirstName { get; set; }

  [ObservableProperty]
  public partial string LastName { get; set; }

  [ObservableProperty]
  public partial string? MiddleName { get ; set; }

  [ObservableProperty]
  public partial string? Position { get; set; }

  [ObservableProperty]
  public partial string? Department { get; set; }

  [ObservableProperty]
  public partial string? Section { get; set; }

  [ObservableProperty]
  public partial string? Type { get; set; }

  [ObservableProperty]
  public partial string? Level { get; set; }

  [ObservableProperty]
  public partial string? Email { get; set; }

  [ObservableProperty]
  public partial string? Image { get; set; }

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
