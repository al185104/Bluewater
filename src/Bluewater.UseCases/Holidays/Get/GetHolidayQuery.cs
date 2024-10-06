using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Holidays.Get;
public record class GetHolidayQuery(Guid HolidayId) : IQuery<Result<HolidayDTO>>;
