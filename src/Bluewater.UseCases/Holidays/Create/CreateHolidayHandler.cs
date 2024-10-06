using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.HolidayAggregate;
using Bluewater.UseCases.Holidays.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateHolidayHandler(IRepository<Holiday> _repository) : ICommandHandler<CreateHolidayCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
  {
    var newHoliday = new Holiday(request.Name, request.Description, request.Date, request.IsRegular);
    var createdItem = await _repository.AddAsync(newHoliday, cancellationToken);
    return createdItem.Id;
  }
}
