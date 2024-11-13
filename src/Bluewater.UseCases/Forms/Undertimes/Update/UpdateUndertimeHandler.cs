using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.UndertimeAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Undertimes.Update;
public class UpdateUndertimeHandler(IRepository<Undertime> _repository) : ICommandHandler<UpdateUndertimeCommand, Result<UndertimeDTO>>
{
  public async Task<Result<UndertimeDTO>> Handle(UpdateUndertimeCommand request, CancellationToken cancellationToken)
  {
    var existingUndertime = await _repository.GetByIdAsync(request.id, cancellationToken);
    if (existingUndertime == null)
    {
      return Result.NotFound();
    }

    existingUndertime.UpdateUndertime(request.empId, request.inclusiveTime, request.reason, request.date, (ApplicationStatus)request.status);

    await _repository.UpdateAsync(existingUndertime, cancellationToken);

    return Result.Success(new UndertimeDTO(existingUndertime.Id, existingUndertime.EmployeeId, $"{existingUndertime.Employee?.LastName}, {existingUndertime.Employee?.FirstName}", existingUndertime.InclusiveTime, existingUndertime.Reason, existingUndertime.Date, (ApplicationStatusDTO)existingUndertime.Status));
  }
}
