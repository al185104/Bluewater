using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.Update;
public record UpdateSingleTimesheetCommand(Guid TimesheetId, Guid EmployeeId, DateTime? time, TimesheetEnum input, DateOnly entryDate) : ICommand<Result<TimesheetDTO>>;


