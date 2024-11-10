using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.ServiceCharges.List;
public record ListServiceChargeQuery(int? skip, int? take, DateOnly date) : IQuery<Result<IEnumerable<ServiceChargeDTO>>>;
