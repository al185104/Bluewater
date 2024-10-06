using Ardalis.Result;

namespace Bluewater.UseCases.Departments.Create;
/// <summary>
/// Create a new Contributor.
/// </summary>
/// <param name="Name"></param>
public record CreateDepartmentCommand(string Name, string? Description, Guid DivisionId) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
