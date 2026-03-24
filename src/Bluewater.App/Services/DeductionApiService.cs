using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class DeductionApiService(IApiClient apiClient) : IDeductionApiService
{
  public async Task<IReadOnlyList<DeductionSummary>> GetDeductionsAsync(
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take, tenant);

    DeductionListResponseDto? response = await apiClient
      .GetAsync<DeductionListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Deductions is not { Count: > 0 })
    {
      return Array.Empty<DeductionSummary>();
    }

    return response.Deductions
      .Where(dto => dto is not null)
      .Select(MapToSummary!)
      .ToList();
  }

  public async Task<DeductionSummary?> CreateDeductionAsync(DeductionSummary deduction, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(deduction);

    if (!deduction.EmpId.HasValue || deduction.EmpId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(deduction));
    }

    CreateDeductionRequestDto request = new()
    {
      EmpId = deduction.EmpId.Value,
      Type = deduction.Type,
      TotalAmount = deduction.TotalAmount,
      MonthlyAmortization = deduction.MonthlyAmortization,
      RemainingBalance = deduction.RemainingBalance,
      NoOfMonths = deduction.NoOfMonths,
      StartDate = deduction.StartDate,
      EndDate = deduction.EndDate,
      Remarks = deduction.Remarks
    };

    CreateDeductionResponseDto? response = await apiClient.PostAsync<CreateDeductionRequestDto, CreateDeductionResponseDto>(
      CreateDeductionRequestDto.Route,
      request,
      cancellationToken).ConfigureAwait(false);

    return response?.Deduction is null ? null : MapToSummary(response.Deduction);
  }

  public async Task<DeductionSummary?> UpdateDeductionAsync(DeductionSummary deduction, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(deduction);

    if (deduction.Id == Guid.Empty)
    {
      throw new ArgumentException("Deduction ID must be provided", nameof(deduction));
    }

    if (!deduction.EmpId.HasValue || deduction.EmpId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(deduction));
    }

    UpdateDeductionRequestDto request = new()
    {
      DeductionId = deduction.Id,
      Id = deduction.Id,
      EmpId = deduction.EmpId.Value,
      Type = deduction.Type,
      TotalAmount = deduction.TotalAmount,
      MonthlyAmortization = deduction.MonthlyAmortization,
      RemainingBalance = deduction.RemainingBalance,
      NoOfMonths = deduction.NoOfMonths,
      StartDate = deduction.StartDate,
      EndDate = deduction.EndDate,
      Remarks = deduction.Remarks,
      Status = deduction.Status
    };

    UpdateDeductionResponseDto? response = await apiClient.PutAsync<UpdateDeductionRequestDto, UpdateDeductionResponseDto>(
      UpdateDeductionRequestDto.BuildRoute(deduction.Id),
      request,
      cancellationToken).ConfigureAwait(false);

    return response?.Deduction is null ? null : MapToSummary(response.Deduction);
  }

  private static string BuildRequestUri(int? skip, int? take, TenantDto tenant)
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

    parameters.Add($"tenant={tenant}");

    string query = string.Join('&', parameters);
    return $"Deductions?{query}";
  }

  private static DeductionSummary MapToSummary(DeductionDto dto)
  {
    ArgumentNullException.ThrowIfNull(dto);

    return new DeductionSummary
    {
      Id = dto.Id,
      EmpId = dto.EmpId,
      Name = dto.Name,
      Type = dto.Type,
      TotalAmount = dto.TotalAmount,
      MonthlyAmortization = dto.MonthlyAmortization,
      RemainingBalance = dto.RemainingBalance,
      NoOfMonths = dto.NoOfMonths,
      StartDate = dto.StartDate,
      EndDate = dto.EndDate,
      Remarks = dto.Remarks,
      Status = dto.Status,
      RowIndex = 0
    };
  }
}
