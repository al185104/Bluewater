using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.UndertimeAggregate;

namespace Bluewater.UseCases.Forms.Undertimes.Create;
public class CreateUndertimeHandler(IRepository<Undertime> _repository) : ICommandHandler<CreateUndertimeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateUndertimeCommand request, CancellationToken cancellationToken)
  {
    var newUndertime = new Undertime(request.empId, request.inclusiveTime, request.reason, request.date);
    var createdItem = await _repository.AddAsync(newUndertime, cancellationToken);
    return createdItem.Id;
  }
}
