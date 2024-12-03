using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.MealCreditAggregate;
using Bluewater.UseCases.MealCredits.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateMealCreditHandler(IRepository<MealCredit> _repository) : ICommandHandler<CreateMealCreditCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateMealCreditCommand request, CancellationToken cancellationToken)
  {
    var newMealCredit = new MealCredit(request.EmployeeId, request.Date, request.Count);
    var createdItem = await _repository.AddAsync(newMealCredit, cancellationToken);
    return createdItem.Id;
  }
}
