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
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Bluewater.App.ViewModels;

public partial class ShiftsViewModel : BaseViewModel
{
  private readonly IShiftApiService shiftApiService;
  private bool hasInitialized;

  public ShiftsViewModel(
    IShiftApiService shiftApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.shiftApiService = shiftApiService;
  }

  public ObservableCollection<ShiftSummary> Shifts { get; } = new();

  [ObservableProperty]
  public partial ShiftSummary? SelectedShift { get; set; }

  public bool CanSaveSelectedShift => SelectedShift is not null && SelectedShift.Id == Guid.Empty;

  public bool CanUpdateSelectedShift => SelectedShift is not null && SelectedShift.Id != Guid.Empty;

  public bool CanDeleteSelectedShift => SelectedShift is not null;

  public override async Task InitializeAsync()
  {
    if (IsBusy || hasInitialized)
    {
      return;
    }

    await LoadShiftsAsync();
  }

  [RelayCommand]
  private async Task AddShift()
  {
    var newShift = new ShiftSummary
    {
      Id = Guid.Empty,
      Name = string.Empty,
      ShiftStartTime = string.Empty,
      ShiftBreakTime = string.Empty,
      ShiftBreakEndTime = string.Empty,
      ShiftEndTime = string.Empty,
      BreakHours = 0m,
      RowIndex = 0
    };

    Shifts.Insert(0, newShift);
    UpdateRowIndices();
    SelectedShift = newShift;

    await TraceCommandAsync("AddShift", new
    {
      ShiftCount = Shifts.Count
    }).ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanSaveSelectedShift))]
  private async Task SaveShiftAsync()
  {
    if (SelectedShift is null)
    {
      return;
    }

    await TraceCommandAsync("SaveShift", new
    {
      ShiftId = SelectedShift.Id,
      SelectedShift.Name
    }).ConfigureAwait(false);

    await PersistShiftAsync(SelectedShift, isNew: true).ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanUpdateSelectedShift))]
  private async Task UpdateShiftAsync()
  {
    if (SelectedShift is null)
    {
      return;
    }

    await TraceCommandAsync("UpdateShift", new
    {
      ShiftId = SelectedShift.Id,
      SelectedShift.Name
    }).ConfigureAwait(false);

    await PersistShiftAsync(SelectedShift, isNew: false).ConfigureAwait(false);
  }

  private async Task PersistShiftAsync(ShiftSummary shift, bool isNew)
  {
    if (IsBusy)
    {
      return;
    }

    try
    {
      IsBusy = true;

      ShiftSummary? result = isNew
        ? await shiftApiService.CreateShiftAsync(shift).ConfigureAwait(false)
        : await shiftApiService.UpdateShiftAsync(shift).ConfigureAwait(false);

      if (result is null)
      {
        return;
      }

      int index = Shifts.IndexOf(shift);

      if (index >= 0)
      {
        result.RowIndex = shift.RowIndex;
        Shifts[index] = result;
        SelectedShift = result;
      }
      else
      {
        result.RowIndex = Shifts.Count;
        Shifts.Add(result);
        SelectedShift = result;
      }

      hasInitialized = false;
      await LoadShiftsAsync(result.Id).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Persisting shift");
    }
    finally
    {
      IsBusy = false;
      RefreshCommandStates();
    }
  }

  private async Task LoadShiftsAsync(Guid? preferredShiftId = null)
  {
    try
    {
      IsBusy = true;
      Guid? targetShiftId = preferredShiftId ?? SelectedShift?.Id;
      SelectedShift = null;
      Shifts.Clear();

      IReadOnlyList<ShiftSummary> shifts = await shiftApiService.GetShiftsAsync().ConfigureAwait(false);

      int index = 0;

      foreach (ShiftSummary shift in shifts.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
      {
        shift.RowIndex = index++;
        Shifts.Add(shift);
      }

      hasInitialized = true;

      if (targetShiftId.HasValue)
      {
        ShiftSummary? matchingShift = Shifts.FirstOrDefault(s => s.Id == targetShiftId.Value);
        SelectedShift = matchingShift ?? (Shifts.Count > 0 ? Shifts[0] : null);
      }
      else
      {
        SelectedShift = Shifts.Count > 0 ? Shifts[0] : null;
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading shifts");
    }
    finally
    {
      IsBusy = false;
      RefreshCommandStates();
    }
  }

  [RelayCommand(CanExecute = nameof(CanDeleteSelectedShift))]
  private async Task DeleteShiftAsync()
  {
    if (SelectedShift is null)
    {
      return;
    }

    ShiftSummary shiftToDelete = SelectedShift;

    bool confirmed = await ConfirmDeleteAsync(shiftToDelete).ConfigureAwait(false);

    if (!confirmed)
    {
      return;
    }

    if (shiftToDelete.Id == Guid.Empty)
    {
      Shifts.Remove(shiftToDelete);
      UpdateRowIndices();
      SelectedShift = Shifts.Count > 0 ? Shifts[0] : null;
      await TraceCommandAsync(nameof(DeleteShiftAsync), new
      {
        shiftToDelete.Id,
        shiftToDelete.Name,
        WasPersisted = false
      }).ConfigureAwait(false);
      return;
    }

    bool deleted = false;

    try
    {
      IsBusy = true;
      deleted = await shiftApiService.DeleteShiftAsync(shiftToDelete.Id).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Deleting shift");
    }
    finally
    {
      IsBusy = false;
    }

    if (!deleted)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeleteShiftAsync), new
    {
      shiftToDelete.Id,
      shiftToDelete.Name,
      WasPersisted = true
    }).ConfigureAwait(false);

    await LoadShiftsAsync().ConfigureAwait(false);
  }

  private static async Task<bool> ConfirmDeleteAsync(ShiftSummary shift)
  {
    return await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      Page? mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;

      if (mainPage is null)
      {
        return false;
      }

      string shiftName = string.IsNullOrWhiteSpace(shift.Name)
        ? "this shift"
        : shift.Name;

      return await mainPage.DisplayAlert(
        "Delete Shift",
        $"Are you sure you want to delete {shiftName}?",
        "Delete",
        "Cancel");
    }).ConfigureAwait(false);
  }

  private void UpdateRowIndices()
  {
    for (int index = 0; index < Shifts.Count; index++)
    {
      Shifts[index].RowIndex = index;
    }
  }

  partial void OnSelectedShiftChanged(ShiftSummary? value)
  {
    RefreshCommandStates();
  }

  private void RefreshCommandStates()
  {
    void refresh()
    {
      OnPropertyChanged(nameof(CanSaveSelectedShift));
      OnPropertyChanged(nameof(CanUpdateSelectedShift));
      OnPropertyChanged(nameof(CanDeleteSelectedShift));
      SaveShiftCommand.NotifyCanExecuteChanged();
      UpdateShiftCommand.NotifyCanExecuteChanged();
      DeleteShiftCommand.NotifyCanExecuteChanged();
    }

    if (MainThread.IsMainThread)
    {
      refresh();
    }
    else
    {
      MainThread.BeginInvokeOnMainThread(refresh);
    }
  }
}
