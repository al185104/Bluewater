using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.UndertimeAggregate;
using Bluewater.Core.UndertimeAggregate.Specifications;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Undertimes.List;
internal class ListUndertimeHandler(IRepository<Undertime> _repository) : IQueryHandler<ListUndertimeQuery, Result<IEnumerable<UndertimeDTO>>>
{
  public async Task<Result<IEnumerable<UndertimeDTO>>> Handle(ListUndertimeQuery request, CancellationToken cancellationToken)
  {
    var spec = new UndertimeAllSpec();
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new UndertimeDTO(s.Id, s.EmployeeId, $"{s.Employee!.LastName}, {s.Employee!.FirstName}", s.InclusiveTime, s.Reason, s.Date, (ApplicationStatusDTO)s.Status)));
  }
}