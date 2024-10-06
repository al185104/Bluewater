using Bluewater.UseCases.Divisions.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Divisions;
/// <summary>
/// Creates a new Division
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateDivisionRequest, CreateDivisionResponse>
{
  public override void Configure()
  {
    Post(CreateDivisionRequest.Route);
    AllowAnonymous();
    Summary(s => { s.ExampleRequest = new CreateDivisionRequest { Name = "Division Name", Description ="Division Description" }; });
  }

  public override async Task HandleAsync(CreateDivisionRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateDivisionCommand(request.Name!,request.Description), cancellationToken);
    if (result.IsSuccess)
    {
      Response = new CreateDivisionResponse(result.Value, request.Name!, request.Description);
      return;
    }
  }
}
