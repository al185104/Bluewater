using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Holidays.Get;
public record GetHolidayQuery(Guid HolidayId) : IQuery<Result<HolidayDTO>>;
