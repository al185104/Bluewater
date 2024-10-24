using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Attendances.List;
public record ListAllAttendancesQuery(int? skip, int? take, DateOnly startDate, DateOnly endDate) : IQuery<Result<IEnumerable<AllAttendancesDTO>>>;
