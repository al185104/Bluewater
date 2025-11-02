using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class AttendanceViewModel : BaseViewModel
{
  private readonly IAttendanceApiService attendanceApiService;
  private readonly IEmployeeApiService employeeApiService;
  private bool hasInitialized;

  public AttendanceViewModel(
    IAttendanceApiService attendanceApiService,
    IEmployeeApiService employeeApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.attendanceApiService = attendanceApiService;
    this.employeeApiService = employeeApiService;
    StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
    EndDate = DateOnly.FromDateTime(DateTime.Today);
    EditableAttendance = CreateNewAttendance();
  }

  public ObservableCollection<AttendanceSummary> Attendances { get; } = new();

  [ObservableProperty]
  public partial Guid? EmployeeFilter { get; set; }

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  [ObservableProperty]
  public partial AttendanceSummary? SelectedAttendance { get; set; }

  [ObservableProperty]
  public partial AttendanceSummary EditableAttendance { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await EnsureDefaultEmployeeFilterAsync().ConfigureAwait(false);
    await LoadAttendancesAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadAttendancesAsync();
  }

  [RelayCommand]
  private void BeginCreateAttendance()
  {
    EditableAttendance = CreateNewAttendance();
    if (EmployeeFilter.HasValue)
    {
      EditableAttendance.EmployeeId = EmployeeFilter.Value;
    }
    SelectedAttendance = null;
  }

  [RelayCommand]
  private void BeginEditAttendance(AttendanceSummary? attendance)
  {
    if (attendance is null)
    {
      return;
    }

    SelectedAttendance = attendance;
    EditableAttendance = CloneAttendance(attendance);
  }

  [RelayCommand]
  private async Task SaveAttendanceAsync()
  {
    if (EditableAttendance is null || EditableAttendance.EmployeeId == Guid.Empty)
    {
      return;
    }

    bool isNew = Attendances.All(attendance => attendance.Id != EditableAttendance.Id);

    try
    {
      IsBusy = true;

      AttendanceSummary? saved = isNew
        ? await attendanceApiService.CreateAttendanceAsync(EditableAttendance)
        : await attendanceApiService.UpdateAttendanceAsync(EditableAttendance);

      AttendanceSummary result = saved ?? EditableAttendance;

      if (isNew)
      {
        Attendances.Add(result);
      }
      else
      {
        int index = FindAttendanceIndex(result.Id);
        if (index >= 0)
        {
          Attendances[index] = result;
        }
        else
        {
          Attendances.Add(result);
        }
      }

      EditableAttendance = CloneAttendance(result);
      await TraceCommandAsync(nameof(SaveAttendanceAsync), result.Id);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, isNew ? "Creating attendance" : "Updating attendance");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task DeleteAttendanceAsync(AttendanceSummary? attendance)
  {
    if (attendance is null)
    {
      return;
    }

    try
    {
      IsBusy = true;

      bool deleted = await attendanceApiService.DeleteAttendanceAsync(attendance.Id);

      if (deleted)
      {
        Attendances.Remove(attendance);
        await TraceCommandAsync(nameof(DeleteAttendanceAsync), attendance.Id);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Deleting attendance");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private async Task LoadAttendancesAsync()
  {
    if (!EmployeeFilter.HasValue || EmployeeFilter.Value == Guid.Empty)
    {
      Attendances.Clear();
      return;
    }

    try
    {
      IsBusy = true;

      IReadOnlyList<AttendanceSummary> attendances = await attendanceApiService
        .GetAttendancesAsync(EmployeeFilter.Value, StartDate, EndDate)
        .ConfigureAwait(false);

      Attendances.Clear();
      foreach (AttendanceSummary attendance in attendances)
      {
        Attendances.Add(attendance);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading attendances");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private async Task EnsureDefaultEmployeeFilterAsync()
  {
    if (EmployeeFilter.HasValue && EmployeeFilter.Value != Guid.Empty)
    {
      return;
    }

    try
    {
      IReadOnlyList<EmployeeSummary> employees = await employeeApiService
        .GetEmployeesAsync(take: 1)
        .ConfigureAwait(false);

      Guid? defaultEmployeeId = employees.FirstOrDefault()?.Id;

      if (defaultEmployeeId.HasValue && defaultEmployeeId.Value != Guid.Empty)
      {
        EmployeeFilter = defaultEmployeeId.Value;
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading default employee");
    }
  }

  private static AttendanceSummary CreateNewAttendance()
  {
    return new AttendanceSummary
    {
      Id = Guid.NewGuid(),
      EntryDate = DateOnly.FromDateTime(DateTime.Today)
    };
  }

  private static AttendanceSummary CloneAttendance(AttendanceSummary attendance)
  {
    return new AttendanceSummary
    {
      Id = attendance.Id,
      EmployeeId = attendance.EmployeeId,
      ShiftId = attendance.ShiftId,
      TimesheetId = attendance.TimesheetId,
      LeaveId = attendance.LeaveId,
      EntryDate = attendance.EntryDate,
      WorkHours = attendance.WorkHours,
      LateHours = attendance.LateHours,
      UnderHours = attendance.UnderHours,
      OverbreakHours = attendance.OverbreakHours,
      NightShiftHours = attendance.NightShiftHours,
      IsLocked = attendance.IsLocked,
      Shift = attendance.Shift,
      Timesheet = attendance.Timesheet
    };
  }

  private int FindAttendanceIndex(Guid attendanceId)
  {
    for (int i = 0; i < Attendances.Count; i++)
    {
      if (Attendances[i].Id == attendanceId)
      {
        return i;
      }
    }

    return -1;
  }
}
