using Ardalis.Result;
namespace Bluewater.UseCases.Pays.Create;

public record CreatePayCommand(decimal basicPay, decimal dailyRate, decimal hourlyRate, decimal hdmfCon, decimal hdmfEr) : Ardalis.SharedKernel.ICommand<Result<Guid>>;