using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.Models;

public enum SettingItemType
{
  Division,
  Department,
  Section,
  Charging,
  Position,
  EmployeeType,
  EmployeeLevel
}

public partial class EditableSettingItem : ObservableObject
{
  public Guid Id { get; set; }

  public SettingItemType Type { get; set; }

  [ObservableProperty]
  public partial string Name { get; set; }

  [ObservableProperty]
  public partial string? Description { get; set; }

  public DivisionSummary ToDivision(int rowIndex)
  {
    return new DivisionSummary
    {
      Id = Id,
      Name = Name,
      Description = Description,
      RowIndex = rowIndex
    };
  }

  public DepartmentSummary ToDepartment(int rowIndex)
  {
    return new DepartmentSummary
    {
      Id = Id,
      Name = Name,
      Description = Description,
      RowIndex = rowIndex
    };
  }

  public SectionSummary ToSection(int rowIndex)
  {
    return new SectionSummary
    {
      Id = Id,
      Name = Name,
      Description = Description,
      RowIndex = rowIndex
    };
  }

  public ChargingSummary ToCharging(int rowIndex)
  {
    return new ChargingSummary
    {
      Id = Id,
      Name = Name,
      Description = Description,
      RowIndex = rowIndex
    };
  }

  public PositionSummary ToPosition(int rowIndex)
  {
    return new PositionSummary
    {
      Id = Id,
      Name = Name,
      Description = Description,
      RowIndex = rowIndex
    };
  }

  public EmployeeTypeSummary ToEmployeeType(bool isActive)
  {
    return new EmployeeTypeSummary
    {
      Id = Id,
      Name = Name,
      Value = string.IsNullOrWhiteSpace(Description) ? Name : Description,
      IsActive = isActive
    };
  }

  public LevelSummary ToEmployeeLevel(bool isActive)
  {
    return new LevelSummary
    {
      Id = Id,
      Name = Name,
      Value = string.IsNullOrWhiteSpace(Description) ? Name : Description,
      IsActive = isActive
    };
  }

  public static EditableSettingItem FromDivision(DivisionSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.Division,
      Name = summary.Name,
      Description = summary.Description
    };
  }

  public static EditableSettingItem FromDepartment(DepartmentSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.Department,
      Name = summary.Name,
      Description = summary.Description
    };
  }

  public static EditableSettingItem FromSection(SectionSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.Section,
      Name = summary.Name,
      Description = summary.Description
    };
  }

  public static EditableSettingItem FromCharging(ChargingSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.Charging,
      Name = summary.Name,
      Description = summary.Description
    };
  }

  public static EditableSettingItem FromPosition(PositionSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.Position,
      Name = summary.Name,
      Description = summary.Description
    };
  }

  public static EditableSettingItem FromEmployeeType(EmployeeTypeSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.EmployeeType,
      Name = summary.Name,
      Description = summary.Value
    };
  }

  public static EditableSettingItem FromEmployeeLevel(LevelSummary summary)
  {
    return new EditableSettingItem
    {
      Id = summary.Id,
      Type = SettingItemType.EmployeeLevel,
      Name = summary.Name,
      Description = summary.Value
    };
  }
}
