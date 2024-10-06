namespace Bluewater.UseCases.Chargings.List;
public interface IListChargingQueryService
{
  Task<IEnumerable<ChargingDTO>> ListAsync();
}
