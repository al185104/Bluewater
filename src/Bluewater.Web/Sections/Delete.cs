using Ardalis.Result;
using Bluewater.UseCases.Sections.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Sections;

/// <summary>
/// Delete a Section
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteSectionRequest>
{
  public override void Configure()
  {
    Delete(DeleteSectionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteSectionRequest request, CancellationToken cancellationToken)
  {
    var command = new DeleteSectionCommand(request.SectionId);

    var result = await _mediator.Send(command, cancellationToken);

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
