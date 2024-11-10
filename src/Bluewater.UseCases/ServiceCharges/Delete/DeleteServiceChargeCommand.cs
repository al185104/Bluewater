using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.ServiceCharges.Delete;
public record DeleteServiceChargeCommand(Guid ServiceChargeId) : ICommand<Result>;
