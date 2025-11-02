using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class ReferenceDataService : IReferenceDataService, IDisposable
{
  private readonly IChargingApiService _chargingApiService;
  private readonly IDivisionApiService _divisionApiService;
  private readonly IDepartmentApiService _departmentApiService;
  private readonly ISectionApiService _sectionApiService;
  private readonly IPositionApiService _positionApiService;
  private readonly IHolidayApiService _holidayApiService;
  private readonly IEmployeeTypeApiService _employeeTypeApiService;
  private readonly ILevelApiService _levelApiService;
  private readonly ILeaveCreditApiService _leaveCreditApiService;

  private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);
  private Task? _initializationTask;

  private IReadOnlyList<ChargingSummary> _chargings = Array.Empty<ChargingSummary>();
  private IReadOnlyList<DivisionSummary> _divisions = Array.Empty<DivisionSummary>();
  private IReadOnlyList<DepartmentSummary> _departments = Array.Empty<DepartmentSummary>();
  private IReadOnlyList<SectionSummary> _sections = Array.Empty<SectionSummary>();
  private IReadOnlyList<PositionSummary> _positions = Array.Empty<PositionSummary>();
  private IReadOnlyList<HolidaySummary> _holidays = Array.Empty<HolidaySummary>();
  private IReadOnlyList<EmployeeTypeSummary> _employeeTypes = Array.Empty<EmployeeTypeSummary>();
  private IReadOnlyList<LevelSummary> _levels = Array.Empty<LevelSummary>();
  private IReadOnlyList<LeaveCreditSummary> _leaveCredits = Array.Empty<LeaveCreditSummary>();

  public ReferenceDataService(
    IChargingApiService chargingApiService,
    IDivisionApiService divisionApiService,
    IDepartmentApiService departmentApiService,
    ISectionApiService sectionApiService,
    IPositionApiService positionApiService,
    IHolidayApiService holidayApiService,
    IEmployeeTypeApiService employeeTypeApiService,
    ILevelApiService levelApiService,
    ILeaveCreditApiService leaveCreditApiService)
  {
    _chargingApiService = chargingApiService;
    _divisionApiService = divisionApiService;
    _departmentApiService = departmentApiService;
    _sectionApiService = sectionApiService;
    _positionApiService = positionApiService;
    _holidayApiService = holidayApiService;
    _employeeTypeApiService = employeeTypeApiService;
    _levelApiService = levelApiService;
    _leaveCreditApiService = leaveCreditApiService;
  }

  public IReadOnlyList<ChargingSummary> Chargings => _chargings;
  public IReadOnlyList<DivisionSummary> Divisions => _divisions;
  public IReadOnlyList<DepartmentSummary> Departments => _departments;
  public IReadOnlyList<SectionSummary> Sections => _sections;
  public IReadOnlyList<PositionSummary> Positions => _positions;
  public IReadOnlyList<HolidaySummary> Holidays => _holidays;
  public IReadOnlyList<EmployeeTypeSummary> EmployeeTypes => _employeeTypes;
  public IReadOnlyList<LevelSummary> Levels => _levels;
  public IReadOnlyList<LeaveCreditSummary> LeaveCredits => _leaveCredits;

  public async Task InitializeAsync(CancellationToken cancellationToken = default)
  {
    Task? initializationTask = _initializationTask;
    if (initializationTask is not null)
    {
      await AwaitInitializationAsync(initializationTask).ConfigureAwait(false);
      return;
    }

    await _initializationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
    try
    {
      if (_initializationTask is null)
      {
        _initializationTask = LoadDataAsync(cancellationToken);
      }

      initializationTask = _initializationTask;
    }
    finally
    {
      _initializationSemaphore.Release();
    }

    await AwaitInitializationAsync(initializationTask!).ConfigureAwait(false);
  }

  private async Task LoadDataAsync(CancellationToken cancellationToken)
  {
    var chargingTask = _chargingApiService.GetChargingsAsync(cancellationToken: cancellationToken);
    var divisionTask = _divisionApiService.GetDivisionsAsync(cancellationToken: cancellationToken);
    var departmentTask = _departmentApiService.GetDepartmentsAsync(cancellationToken: cancellationToken);
    var sectionTask = _sectionApiService.GetSectionsAsync(cancellationToken: cancellationToken);
    var positionTask = _positionApiService.GetPositionsAsync(cancellationToken: cancellationToken);
    var holidayTask = _holidayApiService.GetHolidaysAsync(cancellationToken: cancellationToken);
    var employeeTypeTask = _employeeTypeApiService.GetEmployeeTypesAsync(cancellationToken: cancellationToken);
    var levelTask = _levelApiService.GetLevelsAsync(cancellationToken: cancellationToken);
    var leaveCreditTask = _leaveCreditApiService.GetLeaveCreditsAsync(cancellationToken: cancellationToken);

    await Task.WhenAll(
      chargingTask,
      divisionTask,
      departmentTask,
      sectionTask,
      positionTask,
      holidayTask,
      employeeTypeTask,
      levelTask,
      leaveCreditTask).ConfigureAwait(false);

    _chargings = ApplyRowIndex(chargingTask.Result.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase));
    _divisions = ApplyRowIndex(divisionTask.Result.OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase));
    _departments = ApplyRowIndex(departmentTask.Result.OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase));
    _sections = ApplyRowIndex(sectionTask.Result.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase));
    _positions = ApplyRowIndex(positionTask.Result.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase));
    _holidays = holidayTask.Result.OrderBy(h => h.Date).ToList();
    _employeeTypes = employeeTypeTask.Result.OrderBy(et => et.Name, StringComparer.OrdinalIgnoreCase).ToList();
    _levels = levelTask.Result.OrderBy(l => l.Name, StringComparer.OrdinalIgnoreCase).ToList();
    _leaveCredits = ApplyRowIndex(leaveCreditTask.Result
      .OrderBy(lc => lc.SortOrder)
      .ThenBy(lc => lc.Description, StringComparer.OrdinalIgnoreCase));
  }

  private static IReadOnlyList<T> ApplyRowIndex<T>(IEnumerable<T> source)
    where T : IRowIndexed
  {
    List<T> results = source.ToList();

    for (int i = 0; i < results.Count; i++)
    {
      results[i].RowIndex = i;
    }

    return results;
  }

  private async Task AwaitInitializationAsync(Task initializationTask)
  {
    try
    {
      await initializationTask.ConfigureAwait(false);
    }
    catch
    {
      if (ReferenceEquals(_initializationTask, initializationTask))
      {
        _initializationTask = null;
      }

      throw;
    }
  }

  public void Dispose()
  {
    _initializationSemaphore.Dispose();
  }
}
