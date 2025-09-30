using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LevelAggregate;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class UpdateLevelService(IRepository<Level> _repository, ILogger<UpdateLevelService> _logger) : IUpdateLevelService
{
  public async Task<Result<Level>> UpdateLevelAsync(Guid levelId, string name, string value, bool isActive, CancellationToken cancellationToken = default)
  {
    var existing = await _repository.GetByIdAsync(levelId, cancellationToken);
    if (existing == null)
    {
      return Result.NotFound();
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      return Result<Level>.Invalid(new[] { new ValidationError(nameof(name), "Level name is required.") });
    }

    if (string.IsNullOrWhiteSpace(value))
    {
      return Result<Level>.Invalid(new[] { new ValidationError(nameof(value), "Level value is required.") });
    }

    existing.UpdateLevel(name, value, isActive);
    await _repository.UpdateAsync(existing, cancellationToken);

    _logger.LogInformation("Updated Level {LevelId}", levelId);

    return Result.Success(existing);
  }
}
