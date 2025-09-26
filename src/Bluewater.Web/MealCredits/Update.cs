using Ardalis.Result;
using Bluewater.UseCases.MealCredits;
using Bluewater.UseCases.MealCredits.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.MealCredits;

/// <summary>
/// Update an existing MealCredit record.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateMealCreditRequest, UpdateMealCreditResponse>
{
  public override void Configure()
  {
    Put(UpdateMealCreditRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateMealCreditRequest request, CancellationToken cancellationToken)
  {
    var command = new UpdateMealCreditCommand(request.Id, request.EmployeeId, request.Date, request.Count);
    Result<MealCreditDTO> result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      MealCreditDTO mealCredit = result.Value;
      Response = new UpdateMealCreditResponse(new MealCreditRecord(mealCredit.Id, mealCredit.EmployeeId, mealCredit.Date, mealCredit.Count));
    }
  }
}
