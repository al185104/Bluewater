using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.EmployeeTypeAggregate.Specifications;

namespace Bluewater.UseCases.EmployeeTypes.Get;
public class GetEmployeeTypeHandler(IRepository<EmployeeType> _repository) : IQueryHandler<GetEmployeeTypeQuery, Result<EmployeeTypeDTO>>
{
  public async Task<Result<EmployeeTypeDTO>> Handle(GetEmployeeTypeQuery request, CancellationToken cancellationToken)
  {
    var spec = new EmployeeTypeByIdSpec(request.EmployeeTypeId ?? Guid.Empty);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new EmployeeTypeDTO(entity.Id, entity.Name, entity.Value, entity.IsActive);
  }
}
