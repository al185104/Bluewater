using Ardalis.Result;
using Ardalis.SharedKernel;
using MediatR;
using FastEndpoints;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Web.Users;

public class Get(IRepository<Employee> _empRepository) : Endpoint<GetUserRequest, GetUserResponse>
{
  public override void Configure()
  {
    Get(GetUserRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
  {
    var spec = new EmployeeByBarcodeSpec(req.barcode);
    var emp = await _empRepository.FirstOrDefaultAsync(spec, ct);
    if(emp == null) {
        Response = new GetUserResponse(null);
        return;
    } 
    Response = new GetUserResponse($"{emp.LastName}, {emp.FirstName}");
  }
}
