using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DivisionAggregate;

namespace Bluewater.UseCases.Divisions.Update;
public class UpdateDivisionHandler(IRepository<Division> _repository) : ICommandHandler<UpdateDivisionCommand, Result<DivisionDTO>>
{
  public async Task<Result<DivisionDTO>> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
  {
    var existingDivision = await _repository.GetByIdAsync(request.DivisionId, cancellationToken);
    if (existingDivision == null)
    {
      return Result.NotFound();
    }

    existingDivision.UpdateDivision(request.NewName!, request.Description);

    await _repository.UpdateAsync(existingDivision, cancellationToken);

    return Result.Success(new DivisionDTO(existingDivision.Id, existingDivision.Name, existingDivision.Description ?? string.Empty));
  }
}
