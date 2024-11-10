using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.ServiceCharges.Get;
public record GetServiceChargeQuery(Guid ServiceChargeId) : IQuery<Result<ServiceChargeDTO>>;
