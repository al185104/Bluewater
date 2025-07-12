using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.List;
public record ListOvertimeQuery(int? skip, int? take, Tenant tenant) : IQuery<Result<IEnumerable<OvertimeDTO>>>;
