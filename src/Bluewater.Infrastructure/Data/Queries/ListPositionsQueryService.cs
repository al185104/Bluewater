using Bluewater.UseCases.Positions;
using Bluewater.UseCases.Positions.List;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data.Queries;
public class ListPositionsQueryService(AppDbContext _db) : IListPositionsQueryService
{
  public async Task<IEnumerable<PositionDTO>> ListAsync()
  {
    try
    {
      var ret = await _db.Positions.Select(p => new PositionDTO(p.Id, p.Name, p.Description ?? string.Empty, p.SectionId))
        .ToListAsync();

      return ret.OrderBy(i => i.Name);
    }
    catch (DbUpdateConcurrencyException)
    {
      throw;
    }
  }
}
