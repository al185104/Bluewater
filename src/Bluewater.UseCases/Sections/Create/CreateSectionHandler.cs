using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.SectionAggregate;
using Bluewater.Core.SectionAggregate.Specifications;
using Bluewater.UseCases.Sections.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateSectionHandler(IRepository<Section> _repository) : ICommandHandler<CreateSectionCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
  {
    var existingSection = await _repository.FirstOrDefaultAsync(new SectionByNameSpec(request.Name), cancellationToken);
    if (existingSection != null)
    {
      return existingSection.Id;
    }

    var newSection = new Section(request.Name, request.Description, request.approved1id, request.approved2id, request.approved3id, request.DepartmentId);
    var createdItem = await _repository.AddAsync(newSection, cancellationToken);
    return createdItem.Id;
  }
}
