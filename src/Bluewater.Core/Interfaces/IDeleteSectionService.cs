using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteSectionService
{
  public Task<Result> DeleteSection(Guid sectionId);
}
