using Ardalis.Result;
using Bluewater.UseCases.Sections;
using Bluewater.UseCases.Sections.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Sections;

/// <summary>
/// List all Sections
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<SectionListResponse>
{
  public override void Configure()
  {
    Get("/Sections");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Result<IEnumerable<SectionDTO>> result = await _mediator.Send(new ListSectionsQuery(null, null), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new SectionListResponse
      {
        Sections = result.Value
          .Select(s => new SectionRecord(
            s.Id,
            s.Name,
            s.Description,
            s.Approved1Id,
            s.Approved2Id,
            s.Approved3Id,
            s.DepartmentId))
          .ToList()
      };
    }
  }
}
