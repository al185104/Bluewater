using Ardalis.Result;

namespace Bluewater.UseCases.Chargings.Create;

public record CreateChargingCommand(string Name, string? Description, Guid? DeptId) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
