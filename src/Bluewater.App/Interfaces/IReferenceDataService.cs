using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IReferenceDataService
{
  bool HasInitializationFailed { get; }
  IReadOnlyList<ChargingSummary> Chargings { get; }
  IReadOnlyList<DivisionSummary> Divisions { get; }
  IReadOnlyList<DepartmentSummary> Departments { get; }
  IReadOnlyList<SectionSummary> Sections { get; }
  IReadOnlyList<PositionSummary> Positions { get; }
  IReadOnlyList<HolidaySummary> Holidays { get; }
  IReadOnlyList<EmployeeTypeSummary> EmployeeTypes { get; }
  IReadOnlyList<LevelSummary> Levels { get; }
  IReadOnlyList<LeaveCreditSummary> LeaveCredits { get; }

  Task InitializeAsync(CancellationToken cancellationToken = default);
}
