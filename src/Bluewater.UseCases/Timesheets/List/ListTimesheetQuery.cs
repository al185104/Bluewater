using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Timesheets.List;
public record ListTimesheetQuery(int? skip, int? take, Guid empId, DateOnly startDate, DateOnly endDate) : IQuery<Result<EmployeeTimesheetDTO>>;
