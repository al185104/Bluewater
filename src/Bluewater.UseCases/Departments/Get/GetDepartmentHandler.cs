using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DepartmentAggregate.Specifications;

namespace Bluewater.UseCases.Departments.Get;
public class GetDepartmentHandler(IRepository<Department> _repository) : IQueryHandler<GetDepartmentQuery, Result<DepartmentDTO>>
{
  public async Task<Result<DepartmentDTO>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
  {
    var spec = new DepartmentByIdSpec(request.DepartmentId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new DepartmentDTO(entity.Id, entity.Name, entity.Description ?? string.Empty, entity.DivisionId);
  }
}
