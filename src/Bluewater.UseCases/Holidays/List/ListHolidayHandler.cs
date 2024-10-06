using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.HolidayAggregate;

namespace Bluewater.UseCases.Holidays.List;
// NOTE: CHANGED FROM ORIGINAL IMPLEMENTATION
internal class ListHolidayHandler(IRepository<Holiday> _repository) : IQueryHandler<ListHolidayQuery, Result<IEnumerable<HolidayDTO>>>
{
  public async Task<Result<IEnumerable<HolidayDTO>>> Handle(ListHolidayQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new HolidayDTO(s.Id, s.Name, s.Description, s.Date, s.IsRegular));
    return Result.Success(result);
  }
}
