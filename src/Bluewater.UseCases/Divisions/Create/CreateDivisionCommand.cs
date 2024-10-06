using Ardalis.Result;

namespace Bluewater.UseCases.Divisions.Create;
/// <summary>
/// Create a new Contributor.
/// </summary>
/// <param name="Name"></param>
public record CreateDivisionCommand(string Name, string? Description) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
