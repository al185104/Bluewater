namespace Bluewater.UseCases.Divisions.List;
public interface IListDivisionsQueryService
{
  Task<IEnumerable<DivisionDTO>> ListAsync();
}
