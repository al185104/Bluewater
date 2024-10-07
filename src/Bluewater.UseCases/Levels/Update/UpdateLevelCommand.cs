using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Levels.Update;
public record UpdateLevelCommand(Guid LevelId, string NewName, string Value, bool IsActive) : ICommand<Result<LevelDTO>>;
