using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.PositionAggregate.Specifications;

namespace Bluewater.UseCases.Positions.Get;
public class GetPositionHandler(IRepository<Position> _repository) : IQueryHandler<GetPositionQuery, Result<PositionDTO>>
{
  public async Task<Result<PositionDTO>> Handle(GetPositionQuery request, CancellationToken cancellationToken)
  {
    var spec = new PositionByIdSpec(request.PositionId ?? Guid.Empty);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new PositionDTO(entity.Id, entity.Name, entity.Description ?? string.Empty, entity.SectionId);
  }
}
