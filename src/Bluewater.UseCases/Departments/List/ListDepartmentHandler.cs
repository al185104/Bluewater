using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Departments.List;
internal class ListDepartmentsHandler(IListDepartmentsQueryService _query) : IQueryHandler<ListDepartmentsQuery, Result<IEnumerable<DepartmentDTO>>>
{
  public async Task<Result<IEnumerable<DepartmentDTO>>> Handle(ListDepartmentsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync();
    return Result.Success(result);
  }
}
