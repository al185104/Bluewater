using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Payrolls.List;
public record ListPayrollQuery(int? skip, int? take, string chargingName, DateOnly start, DateOnly end) : IQuery<Result<IEnumerable<PayrollDTO>>>;
