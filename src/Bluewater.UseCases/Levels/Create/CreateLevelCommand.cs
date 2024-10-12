using Ardalis.Result;
namespace Bluewater.UseCases.Levels.Create;

public record CreateLevelCommand(string Name, string Value, bool IsActive) : Ardalis.SharedKernel.ICommand<Result<Guid>>;