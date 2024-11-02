using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OvertimeAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.List;
internal class ListOvertimeHandler(IRepository<Overtime> _repository) : IQueryHandler<ListOvertimeQuery, Result<IEnumerable<OvertimeDTO>>>
{
  public async Task<Result<IEnumerable<OvertimeDTO>>> Handle(ListOvertimeQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new OvertimeDTO(s.Id, s.EmployeeId, s.StartDate, s.EndDate, s.ApprovedHours, s.Remarks, (ApplicationStatusDTO)s.Status));
    return Result.Success(result);
  }
}
