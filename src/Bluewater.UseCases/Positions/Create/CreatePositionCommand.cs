using Ardalis.Result;

namespace Bluewater.UseCases.Positions.Create;
/// <summary>
/// Create a new Contributor.
/// </summary>
/// <param name="Name"></param>
public record CreatePositionCommand(string Name, string? Description, Guid sectionId) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
