using Ardalis.Result;
using Bluewater.UseCases.Sections.Get;
using Bluewater.UseCases.Sections.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Sections;

/// <summary>
/// Update an existing Section
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateSectionRequest, UpdateSectionResponse>
{
  public override void Configure()
  {
    Put(UpdateSectionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateSectionRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new UpdateSectionCommand(
        req.Id,
        req.Name!,
        req.Description,
        req.Approved1Id,
        req.Approved2Id,
        req.Approved3Id,
        req.DepartmentId
      ),
      ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var query = new GetSectionQuery(req.Id);

    var queryResult = await _mediator.Send(query, ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdateSectionResponse(
        new SectionRecord(
          dto.Id,
          dto.Name,
          dto.Description,
          dto.Approved1Id,
          dto.Approved2Id,
          dto.Approved3Id,
          dto.DepartmentId
        )
      );
      return;
    }
  }
}
