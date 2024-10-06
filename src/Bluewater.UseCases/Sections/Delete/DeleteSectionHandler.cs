using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Sections.Delete;
public class DeleteSectionHandler(IDeleteSectionService _deleteSectionService) : ICommandHandler<DeleteSectionCommand, Result>
{
  public async Task<Result> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
  {
    return await _deleteSectionService.DeleteSection(request.SectionId);
  }
}
