using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.UseCases.Positions;
using Bluewater.UseCases.Sections;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Bluewater.UseCases.Positions.Get;
using Bluewater.UseCases.Sections.Get;
using Bluewater.UseCases.Departments;
using Bluewater.UseCases.Departments.Get;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Chargings.Get;

namespace Bluewater.UseCases.Employees.Get;

public class GetEmployeeShortHandler(IReadRepository<Employee> _repository, IServiceScopeFactory _serviceScopeFactory) : IQueryHandler<GetEmployeeShortQuery, Result<EmployeeShortDTO>>
{
  public async Task<Result<EmployeeShortDTO>> Handle(GetEmployeeShortQuery request, CancellationToken cancellationToken)
  {
    var spec = new EmployeeShortByNameSpec(request.EmployeeName);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    PositionDTO? position = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetPositionQuery(entity.PositionId), cancellationToken);
        if (result.IsSuccess)
            position = new PositionDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.SectionId);
        else
            position = new PositionDTO(Guid.Empty, string.Empty, string.Empty, Guid.Empty);
    }

    SectionDTO? section = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetSectionQuery(position!.SectionId), cancellationToken);
        if (result.IsSuccess)
            section = new SectionDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.Approved1Id, result.Value.Approved2Id, result.Value.Approved3Id, result.Value.DepartmentId);
        else
            section = new SectionDTO(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Guid.Empty);
    }

    DepartmentDTO? department = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetDepartmentQuery(section!.DepartmentId), cancellationToken);
        if (result.IsSuccess)
            department = new DepartmentDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.DivisionId);
        else    
            department = new DepartmentDTO(Guid.Empty, string.Empty, string.Empty, Guid.Empty);
    }

    ChargingDTO? charging = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetChargingQuery(entity.ChargingId), cancellationToken);
        if (result.IsSuccess)
            charging = new ChargingDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.DepartmentId);
        else
            charging = new ChargingDTO(Guid.Empty, string.Empty, string.Empty, null);
    }    

    return new EmployeeShortDTO(entity.Id, entity.User?.Username ?? string.Empty, $"{entity.LastName}, {entity.FirstName}", department!.Name, section!.Name, charging!.Name);
  }
}
