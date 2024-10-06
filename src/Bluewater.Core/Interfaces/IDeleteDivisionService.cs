using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteDivisionService
{
  public Task<Result> DeleteDivision(Guid divisionId);
}
