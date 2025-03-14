using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.Delete;
public record DeleteTimesheetCommand(Guid TimesheetId) : ICommand<Result>;
