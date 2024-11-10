using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OtherEarningAggregate;
using Bluewater.Core.OtherEarningAggregate.Specifications;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.List;
internal class ListOtherEarningsByDatesHandler(IRepository<OtherEarning> _repository) : IQueryHandler<ListOtherEarningsByDatesQuery, Result<IEnumerable<OtherEarningDTO>>>
{
  public async Task<Result<IEnumerable<OtherEarningDTO>>> Handle(ListOtherEarningsByDatesQuery request, CancellationToken cancellationToken)
  {
    var spec = new OtherEarningByDatesSpec(request.start, request.end);
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new OtherEarningDTO(s.Id, s.EmployeeId, $"{s.Employee!.LastName}, {s.Employee!.FirstName}", (OtherEarningTypeDTO?)s.EarningType ?? default, s.TotalAmount, s.IsActive, s.Date, (ApplicationStatusDTO)s.Status)));
  }
}
