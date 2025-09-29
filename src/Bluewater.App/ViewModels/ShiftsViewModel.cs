using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class ShiftsViewModel : BaseViewModel
{
  private readonly IShiftApiService shiftApiService;
  private bool hasInitialized;

  public ShiftsViewModel(IShiftApiService shiftApiService)
  {
    this.shiftApiService = shiftApiService;
  }

  public ObservableCollection<ShiftSummary> Shifts { get; } = new();

  [ObservableProperty]
  private ShiftSummary? selectedShift;

  public bool CanSaveSelectedShift => SelectedShift is not null && SelectedShift.Id == Guid.Empty;

  public bool CanUpdateSelectedShift => SelectedShift is not null && SelectedShift.Id != Guid.Empty;

  public override async Task InitializeAsync()
  {
    if (IsBusy || hasInitialized)
    {
      return;
    }

    await LoadShiftsAsync();
  }

  [RelayCommand]
  private void AddShift()
  {
    var newShift = new ShiftSummary
    {
      Id = Guid.Empty,
      Name = string.Empty,
      ShiftStartTime = string.Empty,
      ShiftBreakTime = string.Empty,
      ShiftBreakEndTime = string.Empty,
      ShiftEndTime = string.Empty,
      BreakHours = 0m
    };

    Shifts.Add(newShift);
    SelectedShift = newShift;
  }

  [RelayCommand(CanExecute = nameof(CanSaveSelectedShift))]
  private async Task SaveShiftAsync()
  {
    if (SelectedShift is null)
    {
      return;
    }

    await PersistShiftAsync(SelectedShift, isNew: true);
  }

  [RelayCommand(CanExecute = nameof(CanUpdateSelectedShift))]
  private async Task UpdateShiftAsync()
  {
    if (SelectedShift is null)
    {
      return;
    }

    await PersistShiftAsync(SelectedShift, isNew: false);
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
        ? await shiftApiService.CreateShiftAsync(shift)
        : await shiftApiService.UpdateShiftAsync(shift);

      if (result is null)
      {
        return;
      }

      int index = Shifts.IndexOf(shift);

      if (index >= 0)
      {
        Shifts[index] = result;
        SelectedShift = result;
      }
      else
      {
        Shifts.Add(result);
        SelectedShift = result;
      }

      hasInitialized = false;
      await LoadShiftsAsync(result.Id);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Failed to persist shift: {ex.Message}");
    }
    finally
    {
      IsBusy = false;
      OnPropertyChanged(nameof(CanSaveSelectedShift));
      OnPropertyChanged(nameof(CanUpdateSelectedShift));
      SaveShiftCommand.NotifyCanExecuteChanged();
      UpdateShiftCommand.NotifyCanExecuteChanged();
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

      IReadOnlyList<ShiftSummary> shifts = await shiftApiService.GetShiftsAsync();

      foreach (ShiftSummary shift in shifts.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
      {
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
      Debug.WriteLine($"Failed to load shifts: {ex.Message}");
    }
    finally
    {
      IsBusy = false;
      OnPropertyChanged(nameof(CanSaveSelectedShift));
      OnPropertyChanged(nameof(CanUpdateSelectedShift));
      SaveShiftCommand.NotifyCanExecuteChanged();
      UpdateShiftCommand.NotifyCanExecuteChanged();
    }
  }

  partial void OnSelectedShiftChanged(ShiftSummary? value)
  {
    OnPropertyChanged(nameof(CanSaveSelectedShift));
    OnPropertyChanged(nameof(CanUpdateSelectedShift));
    SaveShiftCommand.NotifyCanExecuteChanged();
    UpdateShiftCommand.NotifyCanExecuteChanged();
  }
}
