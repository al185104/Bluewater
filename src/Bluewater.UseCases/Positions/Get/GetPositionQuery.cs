using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Positions.Get;
public record GetPositionQuery(Guid? PositionId) : IQuery<Result<PositionDTO>>;
