namespace Bluewater.UseCases.Shifts.List;
public interface IListShiftQueryService
{
  Task<IEnumerable<ShiftDTO>> ListAsync();
}
