using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Chargings.List;
public record ListChargingQuery(int? skip, int? take) : IQuery<Result<IEnumerable<ChargingDTO>>>;
