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

public partial class MealCreditViewModel : BaseViewModel
{
  private readonly ILeaveCreditApiService leaveCreditApiService;
  private bool hasInitialized;

  public MealCreditViewModel(
    ILeaveCreditApiService leaveCreditApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.leaveCreditApiService = leaveCreditApiService;
    editableMealCredit = CreateNewMealCredit();
  }

  public ObservableCollection<LeaveCreditSummary> MealCredits { get; } = new();

  [ObservableProperty]
  private LeaveCreditSummary? selectedMealCredit;

  [ObservableProperty]
  private LeaveCreditSummary editableMealCredit;

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadMealCreditsAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadMealCreditsAsync();
  }

  [RelayCommand]
  private void BeginCreateMealCredit()
  {
    EditableMealCredit = CreateNewMealCredit();
    SelectedMealCredit = null;
  }

  [RelayCommand]
  private void BeginEditMealCredit(LeaveCreditSummary? mealCredit)
  {
    if (mealCredit is null)
    {
      return;
    }

    SelectedMealCredit = mealCredit;
    EditableMealCredit = CloneMealCredit(mealCredit);
  }

  [RelayCommand]
  private async Task SaveMealCreditAsync()
  {
    if (string.IsNullOrWhiteSpace(EditableMealCredit.Code))
    {
      return;
    }

    bool isNew = EditableMealCredit.Id == Guid.Empty || MealCredits.All(item => item.Id != EditableMealCredit.Id);
    LeaveCreditSummary item = CloneMealCredit(EditableMealCredit);

    if (isNew)
    {
      item.Id = Guid.NewGuid();
      MealCredits.Add(item);
    }
    else
    {
      int index = FindMealCreditIndex(item.Id);
      if (index >= 0)
      {
        MealCredits[index] = item;
      }
      else
      {
        MealCredits.Add(item);
      }
    }

    EditableMealCredit = item;
    await TraceCommandAsync(nameof(SaveMealCreditAsync), item.Id);
  }

  [RelayCommand]
  private async Task DeleteMealCreditAsync(LeaveCreditSummary? mealCredit)
  {
    if (mealCredit is null)
    {
      return;
    }

    if (MealCredits.Remove(mealCredit))
    {
      await TraceCommandAsync(nameof(DeleteMealCreditAsync), mealCredit.Id);
    }
  }

  private async Task LoadMealCreditsAsync()
  {
    try
    {
      IsBusy = true;

      IReadOnlyList<LeaveCreditSummary> credits = await leaveCreditApiService
        .GetLeaveCreditsAsync()
        .ConfigureAwait(false);

      MealCredits.Clear();
      foreach (LeaveCreditSummary credit in credits)
      {
        MealCredits.Add(credit);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading meal credits");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private static LeaveCreditSummary CreateNewMealCredit()
  {
    return new LeaveCreditSummary
    {
      Id = Guid.Empty,
      Code = string.Empty,
      Description = string.Empty,
      DefaultCredits = 0,
      SortOrder = 0,
      IsLeaveWithPay = true,
      IsCanCarryOver = false
    };
  }

  private static LeaveCreditSummary CloneMealCredit(LeaveCreditSummary mealCredit)
  {
    return new LeaveCreditSummary
    {
      Id = mealCredit.Id,
      Code = mealCredit.Code,
      Description = mealCredit.Description,
      DefaultCredits = mealCredit.DefaultCredits,
      SortOrder = mealCredit.SortOrder,
      IsLeaveWithPay = mealCredit.IsLeaveWithPay,
      IsCanCarryOver = mealCredit.IsCanCarryOver
    };
  }

  private int FindMealCreditIndex(Guid mealCreditId)
  {
    for (int i = 0; i < MealCredits.Count; i++)
    {
      if (MealCredits[i].Id == mealCreditId)
      {
        return i;
      }
    }

    return -1;
  }
}
