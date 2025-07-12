using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Payrolls.List;
public record ListGroupedPayrollQuery(int? skip, int? take) : IQuery<Result<IEnumerable<PayrollSummaryDTO>>>;
