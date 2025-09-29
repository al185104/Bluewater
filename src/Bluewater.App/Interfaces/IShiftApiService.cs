using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IShiftApiService
{
  Task<IReadOnlyList<ShiftSummary>> GetShiftsAsync(CancellationToken cancellationToken = default);
  Task<ShiftSummary?> CreateShiftAsync(ShiftSummary shift, CancellationToken cancellationToken = default);
  Task<ShiftSummary?> UpdateShiftAsync(ShiftSummary shift, CancellationToken cancellationToken = default);
}
