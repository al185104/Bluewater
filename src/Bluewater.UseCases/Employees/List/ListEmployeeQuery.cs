using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Employees.List;
public record ListEmployeeQuery(int? skip, int? take) : IQuery<Result<IEnumerable<EmployeeDTO>>>;
