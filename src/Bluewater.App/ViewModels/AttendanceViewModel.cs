using System;
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
  private bool hasInitialized;

  public AttendanceViewModel(
    IAttendanceApiService attendanceApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.attendanceApiService = attendanceApiService;
    startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
    endDate = DateOnly.FromDateTime(DateTime.Today);
    editableAttendance = CreateNewAttendance();
  }

  public ObservableCollection<AttendanceSummary> Attendances { get; } = new();

  [ObservableProperty]
  private Guid? employeeFilter;

  [ObservableProperty]
  private DateOnly startDate;

  [ObservableProperty]
  private DateOnly endDate;

  [ObservableProperty]
  private AttendanceSummary? selectedAttendance;

  [ObservableProperty]
  private AttendanceSummary editableAttendance;

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
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
    editableAttendance = CreateNewAttendance();
    if (EmployeeFilter.HasValue)
    {
      editableAttendance.EmployeeId = EmployeeFilter.Value;
    }
    selectedAttendance = null;
  }

  [RelayCommand]
  private void BeginEditAttendance(AttendanceSummary? attendance)
  {
    if (attendance is null)
    {
      return;
    }

    selectedAttendance = attendance;
    editableAttendance = CloneAttendance(attendance);
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
