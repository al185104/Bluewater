using System;
using System.Collections.ObjectModel;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.Services;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class SettingViewModel : BaseViewModel
{
  private readonly IDivisionApiService _divisionApiService;
  private readonly IDepartmentApiService _departmentApiService;
  private readonly ISectionApiService _sectionApiService;
  private readonly IChargingApiService _chargingApiService;
  private readonly IPositionApiService _positionApiService;

  public ObservableCollection<DivisionSummary> Divisions { get; } = new();
  public ObservableCollection<DepartmentSummary> Departments { get; } = new();
  public ObservableCollection<SectionSummary> Sections { get; } = new();
  public ObservableCollection<ChargingSummary> Chargings { get; } = new();
  public ObservableCollection<PositionSummary> Positions { get; } = new();

  public SettingViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService,
    IDivisionApiService divisionApiService,
    IDepartmentApiService departmentApiService,
    ISectionApiService sectionApiService,
    IChargingApiService chargingApiService,
    IPositionApiService positionApiService)
    : base(activityTraceService, exceptionHandlingService)
  {
    _divisionApiService = divisionApiService;
    _departmentApiService = departmentApiService;
    _sectionApiService = sectionApiService;
    _chargingApiService = chargingApiService;
    _positionApiService = positionApiService;
  }

  [RelayCommand]
  private async Task EditDivisionAsync(DivisionSummary? division)
  {
    if (division is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditDivisionAsync), division.Id);
  }

  [RelayCommand]
  private async Task DeleteDivisionAsync(DivisionSummary? division)
  {
    if (division is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeleteDivisionAsync), division.Id);
  }

  [RelayCommand]
  private async Task EditDepartmentAsync(DepartmentSummary? department)
  {
    if (department is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditDepartmentAsync), department.Id);
  }

  [RelayCommand]
  private async Task DeleteDepartmentAsync(DepartmentSummary? department)
  {
    if (department is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeleteDepartmentAsync), department.Id);
  }

  [RelayCommand]
  private async Task EditSectionAsync(SectionSummary? section)
  {
    if (section is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditSectionAsync), section.Id);
  }

  [RelayCommand]
  private async Task DeleteSectionAsync(SectionSummary? section)
  {
    if (section is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeleteSectionAsync), section.Id);
  }

  [RelayCommand]
  private async Task EditChargingAsync(ChargingSummary? charging)
  {
    if (charging is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditChargingAsync), charging.Id);
  }

  [RelayCommand]
  private async Task DeleteChargingAsync(ChargingSummary? charging)
  {
    if (charging is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeleteChargingAsync), charging.Id);
  }

  [RelayCommand]
  private async Task EditPositionAsync(PositionSummary? position)
  {
    if (position is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditPositionAsync), position.Id);
  }

  [RelayCommand]
  private async Task DeletePositionAsync(PositionSummary? position)
  {
    if (position is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeletePositionAsync), position.Id);
  }

  public override async Task InitializeAsync()
  {
    if (IsBusy)
    {
      return;
    }

    try
    {
      IsBusy = true;

      Divisions.Clear();
      Departments.Clear();
      Sections.Clear();
      Chargings.Clear();
      Positions.Clear();

      var divisionTask = _divisionApiService.GetDivisionsAsync();
      var departmentTask = _departmentApiService.GetDepartmentsAsync();
      var sectionTask = _sectionApiService.GetSectionsAsync();
      var chargingTask = _chargingApiService.GetChargingsAsync();
      var positionTask = _positionApiService.GetPositionsAsync();

      await Task.WhenAll(divisionTask, departmentTask, sectionTask, chargingTask, positionTask);

      int index = 0;
      foreach (var division in divisionTask.Result.OrderBy(d => d.Name))
      {
        division.RowIndex = index++;
        Divisions.Add(division);
      }

      index = 0;
      foreach (var department in departmentTask.Result.OrderBy(d => d.Name))
      {
        department.RowIndex = index++;
        Departments.Add(department);
      }

      index = 0;
      foreach (var section in sectionTask.Result.OrderBy(s => s.Name))
      {
        section.RowIndex = index++;
        Sections.Add(section);
      }

      index = 0;
      foreach (var charging in chargingTask.Result.OrderBy(c => c.Name))
      {
        charging.RowIndex = index++;
        Chargings.Add(charging);
      }

      index = 0;
      foreach (var position in positionTask.Result.OrderBy(p => p.Name))
      {
        position.RowIndex = index++;
        Positions.Add(position);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading settings data");
    }
    finally
    {
      IsBusy = false;
    }
  }
}
