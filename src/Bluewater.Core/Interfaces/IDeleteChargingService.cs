using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteChargingService
{
  public Task<Result> DeleteCharging(Guid ChargingId);
}
