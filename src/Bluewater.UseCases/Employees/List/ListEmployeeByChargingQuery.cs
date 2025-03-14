using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Employees.List;
public record ListEmployeeByChargingQuery(int? skip, int? take, string chargingName) : IQuery<Result<IEnumerable<EmployeeDTO>>>;
