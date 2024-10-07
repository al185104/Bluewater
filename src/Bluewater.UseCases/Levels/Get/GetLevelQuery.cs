using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Levels.Get;
public record class GetLevelQuery(Guid LevelId) : IQuery<Result<LevelDTO>>;
