using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IDepartmentApiService
{
  Task<IReadOnlyList<DepartmentSummary>> GetDepartmentsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<DepartmentSummary?> GetDepartmentByIdAsync(Guid departmentId, CancellationToken cancellationToken = default);

  Task<DepartmentSummary?> CreateDepartmentAsync(DepartmentSummary department, CancellationToken cancellationToken = default);

  Task<DepartmentSummary?> UpdateDepartmentAsync(DepartmentSummary department, CancellationToken cancellationToken = default);

  Task<bool> DeleteDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
}
