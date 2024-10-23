using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.Get;
public record GetTimesheetQuery(Guid TimesheetId) : IQuery<Result<TimesheetDTO>>;
