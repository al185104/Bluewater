using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Levels.Get;
public record GetLevelQuery(Guid? LevelId) : IQuery<Result<LevelDTO>>;
