using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.EmployeeTypes.List;
public record ListEmployeeTypeQuery(int? skip, int? take) : IQuery<Result<IEnumerable<EmployeeTypeDTO>>>;
