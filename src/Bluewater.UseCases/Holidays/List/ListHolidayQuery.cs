using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Holidays.List;
public record ListHolidayQuery(int? skip, int? take) : IQuery<Result<IEnumerable<HolidayDTO>>>;
