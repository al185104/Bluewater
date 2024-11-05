using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayAggregate;

namespace Bluewater.UseCases.Pays.Update;
public class UpdatePayHandler(IRepository<Pay> _repository) : ICommandHandler<UpdatePayCommand, Result<PayDTO>>
{
  public async Task<Result<PayDTO>> Handle(UpdatePayCommand request, CancellationToken cancellationToken)
  {
    var existingPay = await _repository.GetByIdAsync(request.PayId, cancellationToken);
    if (existingPay == null)
    {
      return Result.NotFound();
    }

    existingPay.UpdatePay(request.basicPay, request.dailyRate, request.hourlyRate, request.hdmfCon, request.hdmfEr, request.cola);

    await _repository.UpdateAsync(existingPay, cancellationToken);

    return Result.Success(new PayDTO(existingPay.Id, existingPay.BasicPay, existingPay.DailyRate, existingPay.HourlyRate, existingPay.HDMF_Con, existingPay.HDMF_Er, existingPay.Cola));
  }
}
