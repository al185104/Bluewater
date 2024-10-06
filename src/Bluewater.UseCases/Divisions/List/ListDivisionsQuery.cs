using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Divisions.List;
public record ListDivisionsQuery(int? skip, int? take) : IQuery<Result<IEnumerable<DivisionDTO>>>;
