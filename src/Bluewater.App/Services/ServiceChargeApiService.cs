using System.Globalization;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class ServiceChargeApiService(IApiClient apiClient) : IServiceChargeApiService
{
  public async Task<IReadOnlyList<ServiceChargeSummary>> GetServiceChargesByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
  {
    string requestUri = $"ServiceCharges?date={date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
    ServiceChargeListResponseDto? response = await apiClient.GetAsync<ServiceChargeListResponseDto>(requestUri, cancellationToken);

    return response?.ServiceCharges?.Select(sc => new ServiceChargeSummary
    {
      Id = sc.Id,
      Username = sc.Username,
      Amount = sc.Amount,
      Date = sc.Date
    }).ToList() ?? [];
  }

  public async Task<Guid?> CreateServiceChargeAsync(ServiceChargeSummary serviceCharge, CancellationToken cancellationToken = default)
  {
    CreateServiceChargeRequestDto request = new()
    {
      Username = serviceCharge.Username,
      Amount = serviceCharge.Amount,
      Date = serviceCharge.Date
    };

    CreateServiceChargeResponseDto? response = await apiClient.PostAsync<CreateServiceChargeRequestDto, CreateServiceChargeResponseDto>(CreateServiceChargeRequestDto.Route, request, cancellationToken);
    return response?.ServiceCharge?.Id;
  }
}
