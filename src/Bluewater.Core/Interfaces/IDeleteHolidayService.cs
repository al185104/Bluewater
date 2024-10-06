using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteHolidayService
{
  public Task<Result> DeleteHoliday(Guid holidayId);
}
