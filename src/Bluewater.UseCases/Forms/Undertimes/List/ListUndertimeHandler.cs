using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.UndertimeAggregate;

namespace Bluewater.UseCases.Forms.Undertimes.List;
internal class ListUndertimeHandler(IRepository<Undertime> _repository) : IQueryHandler<ListUndertimeQuery, Result<IEnumerable<UndertimeDTO>>>
{
  public async Task<Result<IEnumerable<UndertimeDTO>>> Handle(ListUndertimeQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new UndertimeDTO(s.Id, s.EmployeeId, s.InclusiveTime, s.Reason, s.Date, s.Status));
    return Result.Success(result);
  }
}
