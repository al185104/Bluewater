using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Chargings.Update;
public record UpdateChargingCommand(Guid ChargingId, string NewName, string? Description, Guid? deptId) : ICommand<Result<ChargingDTO>>;
