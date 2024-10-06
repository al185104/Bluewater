using Ardalis.Result;

namespace Bluewater.UseCases.Sections.Create;
/// <summary>
/// Create a new Contributor.
/// </summary>
/// <param name="Name"></param>
public record CreateSectionCommand(string Name, string? Description, string? approved1id, string? approved2id, string? approved3id, Guid DepartmentId) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
