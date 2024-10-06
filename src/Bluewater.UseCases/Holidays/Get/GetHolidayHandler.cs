using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.HolidayAggregate;
using Bluewater.Core.HolidayAggregate.Specifications;

namespace Bluewater.UseCases.Holidays.Get;
public class GetHolidayHandler(IRepository<Holiday> _repository) : IQueryHandler<GetHolidayQuery, Result<HolidayDTO>>
{
  public async Task<Result<HolidayDTO>> Handle(GetHolidayQuery request, CancellationToken cancellationToken)
  {
    var spec = new HolidayByIdSpec(request.HolidayId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new HolidayDTO(entity.Id, entity.Name, entity.Description ?? string.Empty, entity.Date, entity.IsRegular);
  }
}
