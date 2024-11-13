using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.OvertimeAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.Update;
public class UpdateOvertimeHandler(IRepository<Overtime> _repository) : ICommandHandler<UpdateOvertimeCommand, Result<OvertimeDTO>>
{
  public async Task<Result<OvertimeDTO>> Handle(UpdateOvertimeCommand request, CancellationToken cancellationToken)
  {
    var existingOvertime = await _repository.GetByIdAsync(request.id, cancellationToken);
    if (existingOvertime == null)
    {
      return Result.NotFound();
    }

    existingOvertime.UpdateOvertime(request.empId, request.startDate, request.endDate, request.approvedHours, request.remarks, (ApplicationStatus)request.status);

    await _repository.UpdateAsync(existingOvertime, cancellationToken);

    return Result.Success(new OvertimeDTO(existingOvertime.Id, existingOvertime.EmployeeId, $"{existingOvertime.Employee?.LastName}, {existingOvertime.Employee?.FirstName}", existingOvertime.StartDate, existingOvertime.EndDate, existingOvertime.ApprovedHours, existingOvertime.Remarks, (ApplicationStatusDTO)existingOvertime.Status));
  }
}
