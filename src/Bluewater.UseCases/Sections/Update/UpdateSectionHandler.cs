using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.SectionAggregate;

namespace Bluewater.UseCases.Sections.Update;
public class UpdateSectionHandler(IRepository<Section> _repository) : ICommandHandler<UpdateSectionCommand, Result<SectionDTO>>
{
  public async Task<Result<SectionDTO>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
  {
    var existingSection = await _repository.GetByIdAsync(request.SectionId, cancellationToken);
    if (existingSection == null)
    {
      return Result.NotFound();
    }

    existingSection.UpdateSection(request.NewName!, request.Description, request.approved1id, request.approved2id, request.approved3id, request.divisionId);

    await _repository.UpdateAsync(existingSection, cancellationToken);

    return Result.Success(new SectionDTO(existingSection.Id, existingSection.Name, existingSection.Description ?? string.Empty, existingSection.Approved1Id, existingSection.Approved2Id, existingSection.Approved3Id, existingSection.DepartmentId));
  }
}
