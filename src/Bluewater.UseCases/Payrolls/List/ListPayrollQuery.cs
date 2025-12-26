using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.UseCases.Common;

namespace Bluewater.UseCases.Payrolls.List;
public record ListPayrollQuery(int? skip, int? take, string? chargingName, DateOnly start, DateOnly end, Tenant tenant) : IQuery<Result<Common.PagedResult<PayrollDTO>>>;
