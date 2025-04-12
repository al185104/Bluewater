using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.LeaveCredits.Delete;

public record DeleteLeaveCreditCommand(Guid LeaveCreditId) : ICommand<Result>;
