using System.Collections.Generic;

namespace Bluewater.UseCases.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
