using Ardalis.Result;

namespace Bluewater.Core.Interfaces;

public interface ICreateEmployeeTypeService
{
  Task<Result<Guid>> CreateEmployeeTypeAsync(string name, string value, bool isActive, CancellationToken cancellationToken = default);
}
