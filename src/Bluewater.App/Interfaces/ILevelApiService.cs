using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface ILevelApiService
{
  Task<IReadOnlyList<LevelSummary>> GetLevelsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);
}
