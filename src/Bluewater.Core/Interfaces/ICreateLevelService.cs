using Ardalis.Result;

namespace Bluewater.Core.Interfaces;

public interface ICreateLevelService
{
  Task<Result<Guid>> CreateLevelAsync(string name, string value, bool isActive, CancellationToken cancellationToken = default);
}
