using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Timesheets.List;
public record ListAllTimesheetQuery(int? skip, int? take, string charging, DateOnly startDate, DateOnly endDate, Tenant tenant) : IQuery<Result<IEnumerable<AllEmployeeTimesheetDTO>>>;
