using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.UseCases.Forms.OtherEarnings.Create;
public class CreateOtherEarningHandler(IRepository<OtherEarning> _repository) : ICommandHandler<CreateOtherEarningCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateOtherEarningCommand request, CancellationToken cancellationToken)
  {
    var newOtherEarning = new OtherEarning(request.empId, (OtherEarningType?)request.type ?? default, request.totalAmount, request.isActive, request.date);
    var createdItem = await _repository.AddAsync(newOtherEarning, cancellationToken);
    return createdItem.Id;
  }
}
