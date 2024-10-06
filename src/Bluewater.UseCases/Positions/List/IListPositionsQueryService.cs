namespace Bluewater.UseCases.Positions.List;
public interface IListPositionsQueryService
{
  Task<IEnumerable<PositionDTO>> ListAsync();
}
