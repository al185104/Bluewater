using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Timesheets.Get;
public class GetTimesheetHandler(IRepository<Timesheet> _repository) : IQueryHandler<GetTimesheetQuery, Result<TimesheetDTO>>
{
  public async Task<Result<TimesheetDTO>> Handle(GetTimesheetQuery request, CancellationToken cancellationToken)
  {
    var spec = new TimesheetByIdSpec(request.TimesheetId);
    var result = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (result == null) return Result.NotFound();

    return Result<TimesheetDTO>.Success(new TimesheetDTO(result!.Id, result.EmployeeId, result.TimeIn1, result.TimeOut1, result.TimeIn2, result.TimeOut2, result.EntryDate, result.IsEdited));
  }
}
