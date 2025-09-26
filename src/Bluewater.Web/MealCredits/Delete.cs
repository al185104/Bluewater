using Ardalis.Result;
using Bluewater.UseCases.MealCredits.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.MealCredits;

/// <summary>
/// Delete a MealCredit entry by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteMealCreditRequest>
{
  public override void Configure()
  {
    Delete(DeleteMealCreditRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteMealCreditRequest request, CancellationToken cancellationToken)
  {
    Result result = await _mediator.Send(new DeleteMealCreditCommand(request.MealCreditId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    }
  }
}
