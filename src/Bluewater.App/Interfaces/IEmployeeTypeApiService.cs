using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IEmployeeTypeApiService
{
  Task<IReadOnlyList<EmployeeTypeSummary>> GetEmployeeTypesAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<EmployeeTypeSummary?> CreateEmployeeTypeAsync(EmployeeTypeSummary employeeType, CancellationToken cancellationToken = default);
}
