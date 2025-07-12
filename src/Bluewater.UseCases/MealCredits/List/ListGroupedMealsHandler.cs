using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.Core.MealCreditAggregate;
using Bluewater.Core.MealCreditAggregate.Specifications;

namespace Bluewater.UseCases.MealCredits.List;
internal class ListGroupedMealsHandler(IRepository<MealCredit> _mealRepository, IRepository<Employee> _empRepository) : IQueryHandler<ListGroupedMealsQuery, Result<IEnumerable<MealCreditsSummaryDTO>>>
{
  public async Task<Result<IEnumerable<MealCreditsSummaryDTO>>> Handle(ListGroupedMealsQuery request, CancellationToken cancellationToken)
  {
    // get employee list by tenant & charging first.
    var empSpec = new EmployeeListSpecByCharging(request.skip, request.take, request.chargingName, request.tenant);
    var employees = await _empRepository.ListAsync(empSpec, cancellationToken);
    if (employees == null) return Result.NotFound();

    // get meal credits for the employees
    var mealSpec = new MealCreditsByDateSpec(employees.Select(e => e.Id).ToList(), request.start, request.end);
    var meals = await _mealRepository.ListAsync(mealSpec, cancellationToken);
    if(meals == null) return Result.NotFound();

    // convert meals to mealcreditssummaryDTO

    // 3) Build summaries
    var summaries = employees
        .Select(emp =>
        {
          // total up this employee's credits (null‐safe)
          var total = meals
              .Where(m => m.EmployeeId == emp.Id)
              .Sum(m => m.Count ?? 0);

          return new MealCreditsSummaryDTO
          {
            EmployeeName = $"{emp.FirstName} {emp.LastName}",
            MealCount = total
          };
        })
        .OrderBy(i => i.EmployeeName)
        .ToList();

    return summaries;
  }
}
