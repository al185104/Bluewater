using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Attendances.Get;
public record GetAllAttendanceQuery(string name, DateOnly startDate, DateOnly endDate) : IQuery<Result<AllAttendancesDTO>>;
