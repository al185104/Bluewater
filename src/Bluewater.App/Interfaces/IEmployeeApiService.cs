using System;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IEmployeeApiService
{
  Task<PagedResult<EmployeeSummary>> GetEmployeesAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<EmployeeSummary?> UpdateEmployeeAsync(
    UpdateEmployeeRequestDto request,
    EmployeeSummary existingSummary,
    CancellationToken cancellationToken = default);

  Task<bool> CreateEmployeeAsync(
    CreateEmployeeRequestDto request,
    CancellationToken cancellationToken = default);

  Task<bool> DeleteEmployeeAsync(
    Guid employeeId,
    CancellationToken cancellationToken = default);
}
