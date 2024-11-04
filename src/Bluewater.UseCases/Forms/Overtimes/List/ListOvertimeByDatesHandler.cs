using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OvertimeAggregate;
using Bluewater.Core.OvertimeAggregate.Specifications;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.List;
internal class ListOvertimeByDatesHandler(IRepository<Overtime> _repository) : IQueryHandler<ListOvertimeByDatesQuery, Result<IEnumerable<OvertimeDTO>>>
{
  public async Task<Result<IEnumerable<OvertimeDTO>>> Handle(ListOvertimeByDatesQuery request, CancellationToken cancellationToken)
  {
    var spec = new OvertimeByDatesSpec(request.start, request.end);
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new OvertimeDTO(s.Id, s.EmployeeId, $"{s.Employee!.LastName}, {s.Employee!.FirstName}", s.StartDate, s.EndDate, s.ApprovedHours, s.Remarks, (ApplicationStatusDTO)s.Status)));
  }
}
