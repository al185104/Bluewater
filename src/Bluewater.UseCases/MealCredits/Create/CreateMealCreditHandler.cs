using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.Core.MealCreditAggregate;
using Bluewater.UseCases.MealCredits.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateMealCreditHandler(IRepository<MealCredit> _repository, IRepository<Employee> _empRepository) : ICommandHandler<CreateMealCreditCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateMealCreditCommand request, CancellationToken cancellationToken)
  {
    var spec = new EmployeeByBarcodeSpec(request.barcode);
    var emp = await _empRepository.FirstOrDefaultAsync(spec, cancellationToken);
    if(emp != null) {
      var newMealCredit = new MealCredit(emp.Id, request.Date, request.Count);
      var createdItem = await _repository.AddAsync(newMealCredit, cancellationToken);
      return createdItem.Id;
    } 
    return Guid.Empty;
  }
}
