using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;

namespace Bluewater.UseCases.EmployeeTypes.List;
// NOTE: CHANGED FROM ORIGINAL IMPLEMENTATION
internal class ListEmployeeTypeHandler(IRepository<EmployeeType> _repository) : IQueryHandler<ListEmployeeTypeQuery, Result<IEnumerable<EmployeeTypeDTO>>>
{
  public async Task<Result<IEnumerable<EmployeeTypeDTO>>> Handle(ListEmployeeTypeQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new EmployeeTypeDTO(s.Id, s.Name, s.Value, s.IsActive));
    return Result.Success(result);
  }
}
