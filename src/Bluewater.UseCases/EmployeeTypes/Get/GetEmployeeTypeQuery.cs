using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.EmployeeTypes.Get;
public record class GetEmployeeTypeQuery(Guid? EmployeeTypeId) : IQuery<Result<EmployeeTypeDTO>>;
