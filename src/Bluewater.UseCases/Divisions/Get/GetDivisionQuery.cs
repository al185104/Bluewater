using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Divisions.Get;
public record class GetDivisionQuery(Guid DivisionId) : IQuery<Result<DivisionDTO>>;
