using System.Data;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Chargings.List;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data.Queries;
public class ListChargingQueryService(AppDbContext _db) : IListChargingQueryService
{
  public async Task<IEnumerable<ChargingDTO>> ListAsync()
  {
    try
    {
      var ret = await _db.Chargings.Select(c => new ChargingDTO(c.Id, c.Name, c.Description ?? string.Empty, c.DepartmentId)).ToListAsync();
      return ret.OrderBy(i => i.Name);
    }
    catch (DBConcurrencyException)
    {
      throw;
    }
  }
}
