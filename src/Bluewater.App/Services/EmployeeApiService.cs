using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class EmployeeApiService(IApiClient apiClient) : IEmployeeApiService
{
  public async Task<IReadOnlyList<EmployeeSummary>> GetEmployeesAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    EmployeeListResponseDto? response = await apiClient.GetAsync<EmployeeListResponseDto>(requestUri, cancellationToken);

    if (response?.Employees is not { Count: > 0 })
    {
      return Array.Empty<EmployeeSummary>();
    }

    return response.Employees
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
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
      return "Employees";
    }

    string query = string.Join('&', parameters);
    return $"Employees?{query}";
  }

  private static EmployeeSummary MapToSummary(EmployeeDto dto)
  {
    return new EmployeeSummary
    {
      Id = dto.Id,
      FirstName = dto.FirstName,
      LastName = dto.LastName,
      MiddleName = dto.MiddleName,
      Department = dto.Department,
      Section = dto.Section,
      Position = dto.Position,
      Type = dto.Type,
      Level = dto.Level,
      Email = dto.ContactInfo?.Email
    };
  }
}
