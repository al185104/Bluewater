using Bluewater.UseCases.Positions.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Positions;

/// <summary>
/// Creates a new Position
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreatePositionRequest, CreatePositionResponse>
{
  public override void Configure()
  {
    Post(CreatePositionRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.ExampleRequest = new CreatePositionRequest
      {
        Name = "Position Name",
        Description = "Position Description",
        SectionId = Guid.NewGuid()
      };
    });
  }

  public override async Task HandleAsync(CreatePositionRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new CreatePositionCommand(request.Name!, request.Description, request.SectionId),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreatePositionResponse(result.Value, request.Name!, request.Description, request.SectionId);
      return;
    }
  }
}
