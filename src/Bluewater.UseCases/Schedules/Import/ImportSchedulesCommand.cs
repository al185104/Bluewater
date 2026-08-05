using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Schedules.Import;

public record ImportSchedulesCommand(Tenant Tenant, IReadOnlyList<ScheduleImportEntryDTO> Entries) : ICommand<Result<ScheduleImportResultDTO>>;

public record ScheduleImportEntryDTO(
  string Barcode,
  DateOnly ScheduleDate,
  string? ShiftName,
  bool IsDefault);

public record ScheduleImportResultDTO(
  int Attempted,
  int Created,
  int Updated,
  int Deleted,
  int SkippedPayrollLocked,
  int SkippedUnchanged,
  int SkippedInvalid);
