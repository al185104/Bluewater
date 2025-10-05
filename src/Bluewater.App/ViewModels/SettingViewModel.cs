using System.Collections.ObjectModel;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.Services;
using Bluewater.App.ViewModels.Base;

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

  public override async Task InitializeAsync()
  {
    if (IsBusy)
      return;

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

      foreach (var division in divisionTask.Result)
      {
        Divisions.Add(division);
      }
      foreach (var department in departmentTask.Result)
      {
        Departments.Add(department);
      }
      foreach (var section in sectionTask.Result)
      {
        Sections.Add(section);
      }
      foreach (var charging in chargingTask.Result)
      {
        Chargings.Add(charging);
      }
      foreach (var position in positionTask.Result)
      {
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
