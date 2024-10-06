using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Positions.Delete;
public record DeletePositionCommand(Guid PositionId) : ICommand<Result>;
