using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OvertimeAggregate;
using Bluewater.Core.OvertimeAggregate.Specifications;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.List;
internal class ListOvertimeHandler(IRepository<Overtime> _repository) : IQueryHandler<ListOvertimeQuery, Result<IEnumerable<OvertimeDTO>>>
{
  public async Task<Result<IEnumerable<OvertimeDTO>>> Handle(ListOvertimeQuery request, CancellationToken cancellationToken)
  {
    var spec = new OvertimeAllSpec(request.tenant);
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new OvertimeDTO(s.Id, s.EmployeeId, $"{s.Employee!.LastName}, {s.Employee!.FirstName}", s.StartDate, s.EndDate, s.ApprovedHours, s.Remarks, (ApplicationStatusDTO)s.Status)));
  }
}
