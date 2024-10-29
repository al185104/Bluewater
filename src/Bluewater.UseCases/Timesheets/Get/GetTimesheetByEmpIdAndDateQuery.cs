using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.Get;
public record GetTimesheetByEmpIdAndDateQuery(Guid EmpId, DateOnly EntryDate) : IQuery<Result<TimesheetDTO>>;
