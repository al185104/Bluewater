using Ardalis.Result;
using Bluewater.UseCases.MealCredits;
using Bluewater.UseCases.MealCredits.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.MealCredits;
/// <summary>
/// List all MealCredits
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<MealCreditListResponse>
{
  public override void Configure()
  {
    Get("/MealCredits");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<MealCreditDTO>> result = await _mediator.Send(new ListMealCreditQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new MealCreditListResponse
      {
        MealCredits = result.Value.Select(d => new MealCreditRecord(d.Id, d.EmployeeId, d.Date, d.Count)).ToList()
      };
    }
  }
}
