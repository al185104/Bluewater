using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UseCases.LeaveCredits;

namespace Bluewater.UseCases.LeaveCredits.Get;

public record GetLeaveCreditQuery(Guid? LeaveCreditId) : IQuery<Result<LeaveCreditDTO>>;
