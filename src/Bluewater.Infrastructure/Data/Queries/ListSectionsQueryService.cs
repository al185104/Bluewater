using System.Data;
using Bluewater.UseCases.Sections;
using Bluewater.UseCases.Sections.List;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data.Queries;
public class ListSectionsQueryService (AppDbContext _db) : IListSectionsQueryService
{
  public async Task<IEnumerable<SectionDTO>> ListAsync()
  {
    try
    {
      var ret = await _db.Sections.Select(s => new SectionDTO(s.Id, s.Name, s.Description ?? string.Empty, s.Approved1Id, s.Approved2Id, s.Approved3Id, s.DepartmentId))
        .ToListAsync();

      return ret.OrderBy(i => i.Name);
    }
    catch (DBConcurrencyException)
    {
      throw;
    }
  }
}
