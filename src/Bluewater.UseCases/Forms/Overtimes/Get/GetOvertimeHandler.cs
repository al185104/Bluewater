using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OvertimeAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.Get;
public class GetOvertimeHandler(IRepository<Overtime> _repository) : IQueryHandler<GetOvertimeQuery, Result<OvertimeDTO>>
{
  public async Task<Result<OvertimeDTO>> Handle(GetOvertimeQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.OvertimeId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new OvertimeDTO(result.Id, result.EmployeeId, $"{result.Employee!.LastName}, {result.Employee!.FirstName}", result.StartDate, result.EndDate, result.ApprovedHours, result.Remarks, (ApplicationStatusDTO)result.Status);
  }
}
