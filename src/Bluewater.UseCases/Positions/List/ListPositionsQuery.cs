using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Positions.List;
public record ListPositionsQuery(int? skip, int? take) : IQuery<Result<IEnumerable<PositionDTO>>>;
