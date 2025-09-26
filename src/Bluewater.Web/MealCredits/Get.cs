using Ardalis.Result;
using Bluewater.UseCases.MealCredits;
using Bluewater.UseCases.MealCredits.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.MealCredits;

/// <summary>
/// Retrieve a single MealCredit by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Get(IMediator _mediator) : Endpoint<GetMealCreditRequest, GetMealCreditResponse>
{
  public override void Configure()
  {
    Get(GetMealCreditRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetMealCreditRequest request, CancellationToken cancellationToken)
  {
    Result<MealCreditDTO> result = await _mediator.Send(new GetMealCreditQuery(request.MealCreditId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      MealCreditDTO mealCredit = result.Value;
      Response = new GetMealCreditResponse(new MealCreditRecord(mealCredit.Id, mealCredit.EmployeeId, mealCredit.Date, mealCredit.Count));
    }
  }
}
