using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.DivisionAggregate.Specifications;

namespace Bluewater.UseCases.Divisions.Get;
public class GetDivisionHandler(IRepository<Division> _repository) : IQueryHandler<GetDivisionQuery, Result<DivisionDTO>>
{
  public async Task<Result<DivisionDTO>> Handle(GetDivisionQuery request, CancellationToken cancellationToken)
  {
    var spec = new DivisionByIdSpec(request.DivisionId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new DivisionDTO(entity.Id, entity.Name, entity.Description ?? string.Empty);
  }
}
