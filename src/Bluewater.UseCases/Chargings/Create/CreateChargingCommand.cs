using Ardalis.Result;

namespace Bluewater.UseCases.Chargings.Create;

public record CreateChargingCommand(string Name, string? Description) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
