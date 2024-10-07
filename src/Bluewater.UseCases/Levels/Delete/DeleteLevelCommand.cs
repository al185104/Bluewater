using Ardalis.Result;
namespace Bluewater.UseCases.Levels.Delete
{
    public record DeleteLevelCommand(Guid LevelId) : Ardalis.SharedKernel.ICommand<Result>;
}