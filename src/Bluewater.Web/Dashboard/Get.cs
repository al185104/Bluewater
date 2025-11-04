using Ardalis.Result;
using Bluewater.UseCases.Dashboards.Home;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Dashboard;

public class Get(IMediator _mediator) : Endpoint<GetHomeDashboardRequest, GetHomeDashboardResponse>
{
  public override void Configure()
  {
    Get("/Dashboard/Home");
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetHomeDashboardRequest request, CancellationToken cancellationToken)
  {
    Result<HomeDashboardDTO> result = await _mediator.Send(new GetHomeDashboardQuery(request.Tenant), cancellationToken);

    if (!result.IsSuccess || result.Value is null)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    Response = new GetHomeDashboardResponse
    {
      Dashboard = HomeDashboardMapper.ToRecord(result.Value)
    };
  }
}
