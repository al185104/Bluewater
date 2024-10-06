using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Positions.Update;
public record UpdatePositionCommand(Guid PositionId, string NewName, string? Description, Guid SectionId) : ICommand<Result<PositionDTO>>;
