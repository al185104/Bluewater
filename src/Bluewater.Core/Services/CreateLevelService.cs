using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LevelAggregate;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class CreateLevelService(IRepository<Level> _repository, ILogger<CreateLevelService> _logger) : ICreateLevelService
{
  public async Task<Result<Guid>> CreateLevelAsync(string name, string value, bool isActive, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return Result<Guid>.Invalid(new[] { new ValidationError(nameof(name), "Level name is required.") });
    }

    if (string.IsNullOrWhiteSpace(value))
    {
      return Result<Guid>.Invalid(new[] { new ValidationError(nameof(value), "Level value is required.") });
    }

    var level = new Level(name, value, isActive);
    var created = await _repository.AddAsync(level, cancellationToken);

    _logger.LogInformation("Created Level {LevelId}", created.Id);

    return Result.Success(created.Id);
  }
}
