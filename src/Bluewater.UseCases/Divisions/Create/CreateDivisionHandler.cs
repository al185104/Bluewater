using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DivisionAggregate;
using Bluewater.UseCases.Divisions.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateDivisionHandler(IRepository<Division> _repository) : ICommandHandler<CreateDivisionCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
  {
    var newDivision = new Division(request.Name, request.Description);
    var createdItem = await _repository.AddAsync(newDivision, cancellationToken);
    return createdItem.Id;
  }
}
