using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayAggregate;

namespace Bluewater.UseCases.Pays.List;

internal class ListPayHandler(IRepository<Pay> _repository) : IQueryHandler<ListPayQuery, Result<IEnumerable<PayDTO>>>
{
  public async Task<Result<IEnumerable<PayDTO>>> Handle(ListPayQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new PayDTO(s.Id, s.BasicPay, s.DailyRate, s.HourlyRate, s.HDMF_Con, s.HDMF_Er));
    return Result.Success(result);
  }
}
