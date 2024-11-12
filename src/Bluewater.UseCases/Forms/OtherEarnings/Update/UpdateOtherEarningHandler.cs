using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.OtherEarningAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.Update;
public class UpdateOtherEarningHandler(IRepository<OtherEarning> _repository) : ICommandHandler<UpdateOtherEarningCommand, Result<OtherEarningDTO>>
{
  public async Task<Result<OtherEarningDTO>> Handle(UpdateOtherEarningCommand request, CancellationToken cancellationToken)
  {
    var existingOtherEarning = await _repository.GetByIdAsync(request.id, cancellationToken);
    if (existingOtherEarning == null)
    {
      return Result.NotFound();
    }

    existingOtherEarning.UpdateOtherEarning(request.empId, (OtherEarningType)request.type, request.totalAmount, request.isActive, request.date, (ApplicationStatus)request.status);

    await _repository.UpdateAsync(existingOtherEarning, cancellationToken);

    return Result.Success(new OtherEarningDTO(existingOtherEarning.Id, existingOtherEarning.EmployeeId, $"{existingOtherEarning.Employee!.LastName},{existingOtherEarning.Employee!.FirstName}", (OtherEarningTypeDTO?)existingOtherEarning.EarningType ?? default, existingOtherEarning.TotalAmount, existingOtherEarning.IsActive, existingOtherEarning.Date, (ApplicationStatusDTO?)existingOtherEarning.Status ?? default));
  }
}
