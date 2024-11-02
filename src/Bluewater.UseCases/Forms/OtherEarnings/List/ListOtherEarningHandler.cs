using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.UseCases.Forms.OtherEarnings.List;
internal class ListOtherEarningHandler(IRepository<OtherEarning> _repository) : IQueryHandler<ListOtherEarningQuery, Result<IEnumerable<OtherEarningDTO>>>
{
  public async Task<Result<IEnumerable<OtherEarningDTO>>> Handle(ListOtherEarningQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new OtherEarningDTO(s.Id, s.EmployeeId, s.TotalAmount, s.IsActive, s.Date, s.Status));
    return Result.Success(result);
  }
}
