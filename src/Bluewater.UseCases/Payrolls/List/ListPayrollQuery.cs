using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Payrolls.List;
public record ListPayrollQuery(int? skip, int? take, string deptName, DateOnly start, DateOnly end) : IQuery<Result<IEnumerable<PayrollDTO>>>;
