using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Departments.List;
public record ListDepartmentsQuery(int? skip, int? take) : IQuery<Result<IEnumerable<DepartmentDTO>>>;
