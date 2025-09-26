using Ardalis.Result;
using Bluewater.UseCases.Users.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Users;

/// <summary>
/// Delete a user by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteUserRequest>
{
  public override void Configure()
  {
    Delete(DeleteUserRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteUserRequest request, CancellationToken cancellationToken)
  {
    Result result = await _mediator.Send(new DeleteUserCommand(request.UserId), cancellationToken);

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
