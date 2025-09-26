using Ardalis.Result;
using Bluewater.UseCases.Users;
using Bluewater.UseCases.Users.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Users;

/// <summary>
/// Retrieve a user by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetUserByIdRequest, GetUserByIdResponse>
{
  public override void Configure()
  {
    Get(GetUserByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetUserByIdRequest request, CancellationToken cancellationToken)
  {
    Result<UserDTO> result = await _mediator.Send(new GetUserQuery(request.UserId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new GetUserByIdResponse(UserMapper.ToRecord(result.Value));
    }
  }
}
