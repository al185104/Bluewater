using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Positions.Get;
public record class GetPositionQuery(Guid PositionId) : IQuery<Result<PositionDTO>>;
