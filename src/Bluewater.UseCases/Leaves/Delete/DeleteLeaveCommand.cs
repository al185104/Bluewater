using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Leaves.Delete;
public record DeleteLeaveCommand(Guid LeaveId) : ICommand<Result>;
