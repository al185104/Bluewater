using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.SectionAggregate;
using Bluewater.Core.SectionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteSectionService(IRepository<Section> _repository, ILogger<DeleteSectionService> _logger) : IDeleteSectionService
{
  public async Task<Result> DeleteSection(Guid SectionId)
  {
    _logger.LogInformation("Deleting Section {contributorId}", SectionId);
    Section? aggregateToDelete = await _repository.GetByIdAsync(SectionId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}
