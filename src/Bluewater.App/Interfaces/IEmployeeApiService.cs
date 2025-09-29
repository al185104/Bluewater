using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IEmployeeApiService
{
  Task<IReadOnlyList<EmployeeSummary>> GetEmployeesAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);
}
