using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Pays.Update;
public record UpdatePayCommand(Guid PayId, decimal basicPay, decimal dailyRate, decimal hourlyRate, decimal hdmfCon, decimal hdmfEr) : ICommand<Result<PayDTO>>;
