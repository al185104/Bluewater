using Ardalis.Result;
using Bluewater.UseCases.Sections.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Sections;

/// <summary>
/// Get a Section by Guid ID
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetSectionByIdRequest, SectionRecord>
{
  public override void Configure()
  {
    Get(GetSectionByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetSectionByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetSectionQuery(request.SectionId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new SectionRecord(
        result.Value.Id,
        result.Value.Name,
        result.Value.Description,
        result.Value.Approved1Id,
        result.Value.Approved2Id,
        result.Value.Approved3Id,
        result.Value.DepartmentId
      );
    }
  }
}
