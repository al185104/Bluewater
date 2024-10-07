using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Levels.List;
public record ListLevelQuery(int? skip, int? take) : IQuery<Result<IEnumerable<LevelDTO>>>;
