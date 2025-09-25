using Bluewater.UseCases.Sections.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Sections;

/// <summary>
/// Creates a new Section
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateSectionRequest, CreateSectionResponse>
{
  public override void Configure()
  {
    Post(CreateSectionRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.ExampleRequest = new CreateSectionRequest
      {
        Name = "Section Name",
        Description = "Section Description",
        DepartmentId = Guid.NewGuid()
      };
    });
  }

  public override async Task HandleAsync(CreateSectionRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new CreateSectionCommand(
        request.Name!,
        request.Description,
        request.Approved1Id,
        request.Approved2Id,
        request.Approved3Id,
        request.DepartmentId
      ),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateSectionResponse(
        result.Value,
        request.Name!,
        request.Description,
        request.Approved1Id,
        request.Approved2Id,
        request.Approved3Id,
        request.DepartmentId
      );
      return;
    }
  }
}
