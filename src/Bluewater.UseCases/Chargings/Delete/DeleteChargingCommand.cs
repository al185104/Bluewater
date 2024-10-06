using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Chargings.Delete;
public record DeleteChargingCommand(Guid ChargingId) : ICommand<Result>;
