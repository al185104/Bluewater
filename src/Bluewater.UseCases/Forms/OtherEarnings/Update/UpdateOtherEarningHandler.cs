using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OtherEarningAggregate;

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

    existingOtherEarning.UpdateOtherEarning(request.empId, request.type, request.totalAmount, request.isActive, request.date, request.status);

    await _repository.UpdateAsync(existingOtherEarning, cancellationToken);

    return Result.Success(new OtherEarningDTO(existingOtherEarning.Id, existingOtherEarning.EmployeeId, existingOtherEarning.TotalAmount, existingOtherEarning.IsActive, existingOtherEarning.Date, existingOtherEarning.Status));
  }
}
