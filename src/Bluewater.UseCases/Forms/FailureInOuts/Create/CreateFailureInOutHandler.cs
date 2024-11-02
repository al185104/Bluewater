using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.FailureInOutAggregate;

namespace Bluewater.UseCases.Forms.FailureInOuts.Create;
public class CreateFailureInOutHandler(IRepository<FailureInOut> _repository) : ICommandHandler<CreateFailureInOutCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateFailureInOutCommand request, CancellationToken cancellationToken)
  {
    var newFailureInOut = new FailureInOut(request.empId, request.date, request.remarks, (FailureInOutReason?)request.reason);
    var createdItem = await _repository.AddAsync(newFailureInOut, cancellationToken);
    return createdItem.Id;
  }
}
