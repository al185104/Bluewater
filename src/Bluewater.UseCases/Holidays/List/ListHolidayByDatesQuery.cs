using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Holidays.List;
public record ListHolidayByDatesQuery(int? skip, int? take, DateOnly start, DateOnly end) : IQuery<Result<IEnumerable<HolidayDTO>>>;
