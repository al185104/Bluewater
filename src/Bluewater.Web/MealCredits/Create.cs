using Bluewater.UseCases.MealCredits.Create;
using Bluewater.Web.MealCredit;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.MealCredits;
/// <summary>
/// Creates a new MealCredit
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateMealCreditRequest, CreateMealCreditResponse>
{
  public override void Configure()
  {
    Post(CreateMealCreditRequest.Route);
    AllowAnonymous();
    Summary(s => { s.ExampleRequest = new CreateMealCreditRequest { barcode = string.Empty, entryDate = DateOnly.MinValue, count = 0 }; });
  }

  public override async Task HandleAsync(CreateMealCreditRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateMealCreditCommand(request.barcode, request.entryDate, request.count), cancellationToken);
    if (result.IsSuccess)
    {
      Response = new CreateMealCreditResponse(result.Value);
      return;
    }
  }
}
