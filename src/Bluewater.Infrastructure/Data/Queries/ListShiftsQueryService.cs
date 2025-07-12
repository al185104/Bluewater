using System.Data;
using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Shifts.List;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data.Queries;
public class ListShiftsQueryService(AppDbContext _db) : IListShiftQueryService
{
  public async Task<IEnumerable<ShiftDTO>> ListAsync()
  {
    try
    {
      var ret = await _db.Shifts.Select(s => new ShiftDTO(s.Id, s.Name, s.ShiftStartTime, s.ShiftBreakTime, s.ShiftBreakEndTime, s.ShiftEndTime, s.BreakHours))
        .ToListAsync();

      return ret.OrderBy(i => i.Name);
    }
    catch (DBConcurrencyException)
    {
      throw;
    }
  }
}
