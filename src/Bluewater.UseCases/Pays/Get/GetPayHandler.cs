using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayAggregate;
using Bluewater.Core.PayAggregate.Specifications;

namespace Bluewater.UseCases.Pays.Get;

public class GetPayHandler(IRepository<Pay> _repository) : IQueryHandler<GetPayQuery, Result<PayDTO>>
{
  public async Task<Result<PayDTO>> Handle(GetPayQuery request, CancellationToken cancellationToken)
  {
    var spec = new PayByIdSpec(request.PayId ?? Guid.Empty);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new PayDTO(entity.Id, entity.BasicPay, entity.DailyRate, entity.HourlyRate, entity.HDMF_Con, entity.HDMF_Er, entity.Cola);
  }
}
