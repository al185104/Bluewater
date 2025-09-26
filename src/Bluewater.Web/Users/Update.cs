using Ardalis.Result;
using Bluewater.UseCases.Users;
using Bluewater.UseCases.Users.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Users;

/// <summary>
/// Update an existing user.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateUserRequest, UpdateUserResponse>
{
  public override void Configure()
  {
    Put(UpdateUserRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateUserRequest request, CancellationToken cancellationToken)
  {
    var command = new UpdateUserCommand(
      request.Id,
      request.Username,
      request.PasswordHash,
      request.Credential,
      request.SupervisedGroup,
      request.IsGlobalSupervisor);

    Result<UserDTO> result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateUserResponse(UserMapper.ToRecord(result.Value));
    }
  }
}
