using Ardalis.Result;

namespace Bluewater.UseCases.ServiceCharges.Create;
public record CreateServiceChargeCommand(string username, decimal svcCharge, DateOnly date) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
