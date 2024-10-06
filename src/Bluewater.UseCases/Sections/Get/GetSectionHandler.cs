using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.SectionAggregate;
using Bluewater.Core.SectionAggregate.Specifications;

namespace Bluewater.UseCases.Sections.Get;
public class GetSectionHandler(IRepository<Section> _repository) : IQueryHandler<GetSectionQuery, Result<SectionDTO>>
{
  public async Task<Result<SectionDTO>> Handle(GetSectionQuery request, CancellationToken cancellationToken)
  {
    var spec = new SectionByIdSpec(request.SectionId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new SectionDTO(entity.Id, entity.Name, entity.Description ?? string.Empty, entity.Approved1Id, entity.Approved2Id, entity.Approved3Id, entity.DepartmentId);
  }
}
