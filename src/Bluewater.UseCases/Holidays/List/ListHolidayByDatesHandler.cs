using System.Text.RegularExpressions;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.HolidayAggregate;
using Bluewater.Core.HolidayAggregate.Specifications;

namespace Bluewater.UseCases.Holidays.List;
internal class ListHolidayByDatesHandler(IRepository<Holiday> _repository) : IQueryHandler<ListHolidayByDatesQuery, Result<IEnumerable<HolidayDTO>>>
{
  public async Task<Result<IEnumerable<HolidayDTO>>> Handle(ListHolidayByDatesQuery request, CancellationToken cancellationToken)
  {

    var spec = new HolidayByDatesSpec(request.start, request.end);
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new HolidayDTO(s.Id, s.Name, s.Description, s.Date, s.IsRegular)));
  }
}
