using Bluewater.UseCases.Schedules.Import;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Schedules;

public class Import(IMediator mediator) : Endpoint<ImportSchedulesRequest, ImportSchedulesResponse>
{
  public override void Configure()
  {
    Post(ImportSchedulesRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(ImportSchedulesRequest request, CancellationToken cancellationToken)
  {
    var command = new ImportSchedulesCommand(
      request.Tenant,
      request.Entries
        .Select(entry => new ScheduleImportEntryDTO(
          entry.Barcode,
          entry.ScheduleDate,
          entry.ShiftName,
          entry.IsDefault))
        .ToList());

    var result = await mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
      return;
    }

    Response = new ImportSchedulesResponse
    {
      Attempted = result.Value.Attempted,
      Created = result.Value.Created,
      Updated = result.Value.Updated,
      Deleted = result.Value.Deleted,
      SkippedPayrollLocked = result.Value.SkippedPayrollLocked,
      SkippedUnchanged = result.Value.SkippedUnchanged,
      SkippedInvalid = result.Value.SkippedInvalid
    };
  }
}
