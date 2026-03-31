using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IHolidayApiService
{
  Task<IReadOnlyList<HolidaySummary>> GetHolidaysAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<HolidaySummary?> CreateHolidayAsync(HolidaySummary holiday, CancellationToken cancellationToken = default);

  Task<HolidaySummary?> UpdateHolidayAsync(HolidaySummary holiday, CancellationToken cancellationToken = default);

  Task<bool> DeleteHolidayAsync(Guid holidayId, CancellationToken cancellationToken = default);
}
