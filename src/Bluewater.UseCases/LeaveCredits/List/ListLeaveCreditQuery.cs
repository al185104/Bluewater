
using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.LeaveCredits.List;

public record ListLeaveCreditQuery(int? skip, int? take) : IQuery<Result<IEnumerable<LeaveCreditDTO>>>;
