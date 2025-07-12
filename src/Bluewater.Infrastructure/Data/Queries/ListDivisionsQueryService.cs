using Bluewater.UseCases.Divisions;
using Bluewater.UseCases.Divisions.List;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data.Queries;
public class ListDivisionsQueryService(AppDbContext _db) : IListDivisionsQueryService
{
  public async Task<IEnumerable<DivisionDTO>> ListAsync()
  {
    try
    {
      var ret = await _db.Divisions.Select(d => new DivisionDTO(d.Id, d.Name, d.Description ?? string.Empty))
        .ToListAsync();

      return ret.OrderBy(i => i.Name);
    }
    catch (DbUpdateConcurrencyException)
    {
      throw;
    }
  }
}
