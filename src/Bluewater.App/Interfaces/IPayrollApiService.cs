using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IPayrollApiService
{
  Task<IReadOnlyList<PayrollSummary>> GetPayrollsAsync(
    DateOnly startDate,
    DateOnly endDate,
    string? chargingName = null,
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default);

  Task<IReadOnlyList<PayrollGroupedSummary>> GetGroupedPayrollsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<PayrollSummary?> GetPayrollByIdAsync(Guid payrollId, CancellationToken cancellationToken = default);

  Task<Guid?> CreatePayrollAsync(PayrollSummary payroll, CancellationToken cancellationToken = default);

  Task<PayrollSummary?> UpdatePayrollAsync(PayrollSummary payroll, CancellationToken cancellationToken = default);

  Task<bool> DeletePayrollAsync(Guid payrollId, CancellationToken cancellationToken = default);
}
