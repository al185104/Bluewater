using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.List;
public record ListTimesheetQuery(int? skip, int? take, string name, DateOnly startDate, DateOnly endDate) : IQuery<Result<EmployeeTimesheetDTO>>;
