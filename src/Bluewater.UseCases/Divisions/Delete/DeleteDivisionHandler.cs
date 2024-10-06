using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Divisions.Delete;
public class DeleteDivisionHandler(IDeleteDivisionService _deleteDivisionService) : ICommandHandler<DeleteDivisionCommand, Result>
{
  public async Task<Result> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
  {
    return await _deleteDivisionService.DeleteDivision(request.DivisionId);
  }
}
