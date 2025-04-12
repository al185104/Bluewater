using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Leaves.List;
public record ListLeaveQuery(int? skip, int? take) : IQuery<Result<IEnumerable<LeaveDTO>>>;
