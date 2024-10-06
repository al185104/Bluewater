using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.HolidayAggregate;

namespace Bluewater.UseCases.Holidays.Update;
public class UpdateHolidayHandler(IRepository<Holiday> _repository) : ICommandHandler<UpdateHolidayCommand, Result<HolidayDTO>>
{
  public async Task<Result<HolidayDTO>> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
  {
    var existingHoliday = await _repository.GetByIdAsync(request.HolidayId, cancellationToken);
    if (existingHoliday == null)
    {
      return Result.NotFound();
    }

    existingHoliday.UpdateHoliday(request.NewName!, request.Description, request.Date,  request.IsRegular);

    await _repository.UpdateAsync(existingHoliday, cancellationToken);

    return Result.Success(new HolidayDTO(existingHoliday.Id, existingHoliday.Name, existingHoliday.Description ?? string.Empty, existingHoliday.Date, existingHoliday.IsRegular));
  }
}
