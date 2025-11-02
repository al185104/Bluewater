using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class LeaveViewModel : BaseViewModel
{
  private readonly ILeaveApiService leaveApiService;
  private bool hasInitialized;

  public LeaveViewModel(
    ILeaveApiService leaveApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.leaveApiService = leaveApiService;
    EditableLeave = CreateNewLeave();
  }

  public ObservableCollection<LeaveSummary> Leaves { get; } = new();

  [ObservableProperty]
  public partial LeaveSummary? SelectedLeave { get; set; }

  [ObservableProperty]
  public partial LeaveSummary EditableLeave { get; set; }

  [ObservableProperty]
  public partial TenantDto TenantFilter { get; set; } = TenantDto.Maribago;

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadLeavesAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadLeavesAsync();
  }

  [RelayCommand]
  private void BeginCreateLeave()
  {
    EditableLeave = CreateNewLeave();
    SelectedLeave = null;
  }

  [RelayCommand]
  private void BeginEditLeave(LeaveSummary? leave)
  {
    if (leave is null)
    {
      return;
    }

    SelectedLeave = leave;
    EditableLeave = CloneLeave(leave);
  }

  [RelayCommand]
  private async Task SaveLeaveAsync()
  {
    if (EditableLeave.EmployeeId is null || EditableLeave.EmployeeId.Value == Guid.Empty)
    {
      return;
    }

    bool isNew = EditableLeave.Id == Guid.Empty;

    try
    {
      IsBusy = true;

      LeaveSummary? saved = isNew
        ? await leaveApiService.CreateLeaveAsync(EditableLeave)
        : await leaveApiService.UpdateLeaveAsync(EditableLeave);

      LeaveSummary result = saved ?? EditableLeave;

      if (isNew)
      {
        result.RowIndex = Leaves.Count;
        Leaves.Add(result);
      }
      else
      {
        int index = FindLeaveIndex(result.Id);
        if (index >= 0)
        {
          result.RowIndex = index;
          Leaves[index] = result;
        }
        else
        {
          result.RowIndex = Leaves.Count;
          Leaves.Add(result);
        }
      }

      UpdateLeaveRowIndexes();
      EditableLeave = CloneLeave(result);
      await TraceCommandAsync(nameof(SaveLeaveAsync), result.Id);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, isNew ? "Creating leave" : "Updating leave");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task DeleteLeaveAsync(LeaveSummary? leave)
  {
    if (leave is null)
    {
      return;
    }

    try
    {
      IsBusy = true;

      bool deleted = await leaveApiService.DeleteLeaveAsync(leave.Id);

      if (deleted)
      {
        Leaves.Remove(leave);
        UpdateLeaveRowIndexes();
        await TraceCommandAsync(nameof(DeleteLeaveAsync), leave.Id);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Deleting leave");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private async Task LoadLeavesAsync()
  {
    try
    {
      IsBusy = true;

      IReadOnlyList<LeaveSummary> leaves = await leaveApiService
        .GetLeavesAsync(tenant: TenantFilter)
        .ConfigureAwait(false);

      Leaves.Clear();
      foreach (LeaveSummary leave in leaves)
      {
        leave.RowIndex = Leaves.Count;
        Leaves.Add(leave);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading leaves");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private static LeaveSummary CreateNewLeave()
  {
    return new LeaveSummary
    {
      Id = Guid.Empty,
      StartDate = DateTime.Today,
      EndDate = DateTime.Today,
      RowIndex = 0
    };
  }

  private static LeaveSummary CloneLeave(LeaveSummary leave)
  {
    return new LeaveSummary
    {
      Id = leave.Id,
      StartDate = leave.StartDate,
      EndDate = leave.EndDate,
      IsHalfDay = leave.IsHalfDay,
      Status = leave.Status,
      EmployeeId = leave.EmployeeId,
      LeaveCreditId = leave.LeaveCreditId,
      EmployeeName = leave.EmployeeName,
      LeaveCreditName = leave.LeaveCreditName,
      RowIndex = leave.RowIndex
    };
  }

  private int FindLeaveIndex(Guid leaveId)
  {
    for (int i = 0; i < Leaves.Count; i++)
    {
      if (Leaves[i].Id == leaveId)
      {
        return i;
      }
    }

    return -1;
  }

  private void UpdateLeaveRowIndexes()
  {
    Leaves.UpdateRowIndexes();
  }
}
