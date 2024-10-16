using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Chargings.Get;
public record GetChargingQuery(Guid? ChargingId) : IQuery<Result<ChargingDTO>>;
