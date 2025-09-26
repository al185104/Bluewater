using Ardalis.Result;
using Bluewater.UseCases.Users.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Users;

/// <summary>
/// Create a new application user.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateUserRequest, CreateUserResponse>
{
  public override void Configure()
  {
    Post(CreateUserRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
  {
    var command = new CreateUserCommand(request.Username, request.PasswordHash, request.Credential, request.SupervisedGroup, request.IsGlobalSupervisor);

    Result<Guid> result = await _mediator.Send(command, cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateUserResponse(result.Value);
    }
  }
}
