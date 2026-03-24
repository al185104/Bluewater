using Ardalis.Result;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using Bluewater.UseCases.Forms.Deductions;
using Bluewater.UseCases.Forms.Deductions.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Deductions;

/// <summary>
/// Lists deductions.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<DeductionListRequest, DeductionListResponse>
{
  public override void Configure()
  {
    Get("/Deductions");
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeductionListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<DeductionDTO>> result = await _mediator.Send(
      new ListDeductionQuery(request.Skip, request.Take, request.Tenant),
      cancellationToken);

    if (!result.IsSuccess)
    {
      return;
    }

    IEnumerable<DeductionDTO> deductions = result.Value;

    if (request.ChargingId.HasValue && request.ChargingId.Value != Guid.Empty)
    {
      Result<Bluewater.UseCases.Common.PagedResult<EmployeeDTO>> employees = await _mediator.Send(
        new ListEmployeeQuery(null, null, request.Tenant),
        cancellationToken);

      if (employees.IsSuccess)
      {
        HashSet<Guid> employeeIds = employees.Value.Items
          .Where(item => item.ChargingId == request.ChargingId)
          .Select(item => item.Id)
          .ToHashSet();

        deductions = deductions.Where(item => item.EmpId.HasValue && employeeIds.Contains(item.EmpId.Value));
      }
      else
      {
        deductions = [];
      }
    }

    Response = new DeductionListResponse
    {
      Deductions = deductions.Select(DeductionMapper.ToRecord).ToList()
    };
  }
}
