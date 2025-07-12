using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Employees.Get;


public class GetEmployeeByBarcodeHandler(IRepository<Employee> _repository, IServiceScopeFactory _serviceScopeFactory) : IQueryHandler<GetEmployeeByBarcodeQuery, Result<EmployeeShortDTO>>
{
    public async Task<Result<EmployeeShortDTO>> Handle(GetEmployeeByBarcodeQuery request, CancellationToken cancellationToken) {
        
        var spec = new EmployeeByBarcodeSpec(request.barcode);
        var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        if (entity == null) return Result.NotFound();

        using(var scope = _serviceScopeFactory.CreateScope()){
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var ret = await mediator.Send(new GetEmployeeShortQuery($"{entity.LastName}, {entity.FirstName}"));
            if (ret.IsSuccess)
                return ret.Value;
        }

        return Result.NotFound();
    }
}
