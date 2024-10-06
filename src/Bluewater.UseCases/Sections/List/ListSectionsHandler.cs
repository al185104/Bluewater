using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Sections.List;
public class ListSectionsHandler(IListSectionsQueryService _query) : IQueryHandler<ListSectionsQuery, Result<IEnumerable<SectionDTO>>>
{
  public async Task<Result<IEnumerable<SectionDTO>>> Handle(ListSectionsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync();
    return Result.Success(result);
  }
}
