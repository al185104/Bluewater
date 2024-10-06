using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Holidays.Delete;
public class DeleteHolidayHandler(IDeleteHolidayService _deleteHolidayService) : ICommandHandler<DeleteHolidayCommand, Result>
{
  public async Task<Result> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
  {
    return await _deleteHolidayService.DeleteHoliday(request.HolidayId);
  }
}
