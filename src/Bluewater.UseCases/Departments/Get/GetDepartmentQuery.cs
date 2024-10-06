using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Departments.Get;
public record class GetDepartmentQuery(Guid DepartmentId) : IQuery<Result<DepartmentDTO>>;
