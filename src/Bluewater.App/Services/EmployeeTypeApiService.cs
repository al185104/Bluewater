using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class EmployeeTypeApiService(IApiClient apiClient) : IEmployeeTypeApiService
{
  public async Task<IReadOnlyList<EmployeeTypeSummary>> GetEmployeeTypesAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    EmployeeTypeListResponseDto? response = await apiClient
      .GetAsync<EmployeeTypeListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.EmployeeTypes is not { Count: > 0 })
    {
      return Array.Empty<EmployeeTypeSummary>();
    }

    return response.EmployeeTypes
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<EmployeeTypeSummary?> CreateEmployeeTypeAsync(EmployeeTypeSummary employeeType, CancellationToken cancellationToken = default)
  {
    if (employeeType is null)
    {
      throw new ArgumentNullException(nameof(employeeType));
    }

    CreateEmployeeTypeRequestDto request = new()
    {
      Name = employeeType.Name,
      Value = employeeType.Value,
      IsActive = employeeType.IsActive
    };

    EmployeeTypeDto? response = await apiClient
      .PostAsync<CreateEmployeeTypeRequestDto, EmployeeTypeDto>("EmployeeTypes", request, cancellationToken)
      .ConfigureAwait(false);

    return response is null ? null : MapToSummary(response);
  }


  public async Task<EmployeeTypeSummary?> UpdateEmployeeTypeAsync(EmployeeTypeSummary employeeType, CancellationToken cancellationToken = default)
  {
    if (employeeType is null)
    {
      throw new ArgumentNullException(nameof(employeeType));
    }

    UpdateEmployeeTypeRequestDto request = new()
    {
      Id = employeeType.Id,
      Name = employeeType.Name,
      Value = employeeType.Value,
      IsActive = employeeType.IsActive
    };

    UpdateEmployeeTypeResponseDto? response = await apiClient
      .PutAsync<UpdateEmployeeTypeRequestDto, UpdateEmployeeTypeResponseDto>("EmployeeTypes", request, cancellationToken)
      .ConfigureAwait(false);

    return response?.EmployeeType is null ? null : MapToSummary(response.EmployeeType);
  }

  public Task<bool> DeleteEmployeeTypeAsync(Guid employeeTypeId, CancellationToken cancellationToken = default)
  {
    if (employeeTypeId == Guid.Empty)
    {
      throw new ArgumentException("Employee type ID must be provided", nameof(employeeTypeId));
    }

    return apiClient.DeleteAsync($"EmployeeTypes/{employeeTypeId}", cancellationToken);
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
      return "EmployeeTypes";
    }

    string query = string.Join('&', parameters);
    return $"EmployeeTypes?{query}";
  }

  private static EmployeeTypeSummary MapToSummary(EmployeeTypeDto? dto)
  {
    if (dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new EmployeeTypeSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Value = dto.Value,
      IsActive = dto.IsActive
    };
  }
}
