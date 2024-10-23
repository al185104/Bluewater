using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.List;
public record ListAllTimesheetQuery(int? skip, int? take, DateOnly startDate, DateOnly endDate) : IQuery<Result<IEnumerable<AllEmployeeTimesheetDTO>>>;
