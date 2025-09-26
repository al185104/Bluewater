using System.Linq;
using Ardalis.Result;
using Bluewater.UseCases.Users;
using Bluewater.UseCases.Users.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Users;

/// <summary>
/// List application users with optional paging.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<UserListRequest, UserListResponse>
{
  public override void Configure()
  {
    Get("/Users");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UserListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<UserDTO>> result = await _mediator.Send(new ListUserQuery(request.Skip, request.Take), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new UserListResponse
      {
        Users = result.Value.Select(UserMapper.ToRecord).ToList()
      };
    }
  }
}
