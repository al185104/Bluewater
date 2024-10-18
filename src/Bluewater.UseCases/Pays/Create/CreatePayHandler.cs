using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayAggregate;
using Bluewater.UseCases.Pays.Create;

namespace Bluewater.UseCases.Pays.Create;
public class CreatePayHandler(IRepository<Pay> _repository) : ICommandHandler<CreatePayCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreatePayCommand request, CancellationToken cancellationToken)
  {
    var newPay = new Pay(request.basicPay, request.dailyRate, request.hourlyRate, request.hdmfCon, request.hdmfEr);
    var createdItem = await _repository.AddAsync(newPay, cancellationToken);
    return createdItem.Id;
  }
}
