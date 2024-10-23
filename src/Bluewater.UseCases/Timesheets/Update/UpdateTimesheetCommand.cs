using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.Update;
public record UpdateTimesheetCommand(Guid TimesheetId, Guid employeeId, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly entryDate) : ICommand<Result<TimesheetDTO>>;
