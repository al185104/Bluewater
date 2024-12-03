using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.MealCreditAggregate;
using Bluewater.UseCases.MealCredits;
using Bluewater.UseCases.MealCredits.List;

namespace Bluewater.UseCases.Contributors.Create;
public class ListMealCreditHandler(IRepository<MealCredit> _repository) : IQueryHandler<ListMealCreditQuery, Result<IEnumerable<MealCreditDTO>>>
{
  public async Task<Result<IEnumerable<MealCreditDTO>>> Handle(ListMealCreditQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new MealCreditDTO(s.Id, s.EmployeeId, s.Date, s.Count));
    return Result.Success(result);    
  }
}
