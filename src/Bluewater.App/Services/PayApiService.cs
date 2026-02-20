using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class PayApiService(IApiClient apiClient) : IPayApiService
{
  public async Task<IReadOnlyList<PayRecordDto>> GetPaysAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    PayListResponseDto? response = await apiClient
      .GetAsync<PayListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Pays is not { Count: > 0 })
    {
      return Array.Empty<PayRecordDto>();
    }

    return response.Pays
      .Where(pay => pay is not null)
      .ToList();
  }

  public Task<PayRecordDto?> GetPayByIdAsync(Guid payId, CancellationToken cancellationToken = default)
  {
    if (payId == Guid.Empty)
    {
      throw new ArgumentException("Pay ID must be provided", nameof(payId));
    }

    return apiClient.GetAsync<PayRecordDto>($"Pays/{payId}", cancellationToken);
  }

  public async Task<PayRecordDto?> CreatePayAsync(PayRecordDto pay, CancellationToken cancellationToken = default)
  {
    if (pay is null)
    {
      throw new ArgumentNullException(nameof(pay));
    }

    CreatePayRequestDto request = new()
    {
      BasicPay = pay.BasicPay ?? 0,
      DailyRate = pay.DailyRate ?? 0,
      HourlyRate = pay.HourlyRate ?? 0,
      HdmfCon = pay.HdmfEmployeeContribution ?? 0,
      HdmfEr = pay.HdmfEmployerContribution ?? 0
    };

    CreatePayResponseDto? response = await apiClient
      .PostAsync<CreatePayRequestDto, CreatePayResponseDto>(CreatePayRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    return response?.Pay is null
      ? null
      : await GetPayByIdAsync(response.Pay.Id, cancellationToken).ConfigureAwait(false);
  }

  public async Task<PayRecordDto?> UpdatePayAsync(PayRecordDto pay, CancellationToken cancellationToken = default)
  {
    if (pay is null)
    {
      throw new ArgumentNullException(nameof(pay));
    }

    if (pay.Id == Guid.Empty)
    {
      throw new ArgumentException("Pay ID must be provided", nameof(pay));
    }

    UpdatePayRequestDto request = new()
    {
      PayId = pay.Id,
      BasicPay = pay.BasicPay ?? 0,
      DailyRate = pay.DailyRate ?? 0,
      HourlyRate = pay.HourlyRate ?? 0,
      HdmfCon = pay.HdmfEmployeeContribution ?? 0,
      HdmfEr = pay.HdmfEmployerContribution ?? 0,
      Cola = pay.Cola ?? 0
    };

    UpdatePayResponseDto? response = await apiClient
      .PutAsync<UpdatePayRequestDto, UpdatePayResponseDto>(
        UpdatePayRequestDto.BuildRoute(pay.Id),
        request,
        cancellationToken)
      .ConfigureAwait(false);

    return response?.Pay;
  }

  public Task<bool> DeletePayAsync(Guid payId, CancellationToken cancellationToken = default)
  {
    if (payId == Guid.Empty)
    {
      throw new ArgumentException("Pay ID must be provided", nameof(payId));
    }

    return apiClient.DeleteAsync(UpdatePayRequestDto.BuildRoute(payId), cancellationToken);
  }

  private static string BuildRequestUri(int? skip, int? take)
  {
    List<string> parameters = [];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    if (parameters.Count == 0)
    {
      return "Pays";
    }

    string query = string.Join('&', parameters);
    return $"Pays?{query}";
  }
}
