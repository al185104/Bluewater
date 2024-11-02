using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.FailureInOuts.Delete;
public record DeleteFailureInOutCommand(Guid FailureInOutId) : ICommand<Result>;
